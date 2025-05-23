using System.Text.Json;
using Asp.Versioning.ApiExplorer;
using BookingsApi.DAL.Services;
using BookingsApi.Infrastructure.Services.AsynchronousProcesses;
using BookingsApi.Infrastructure.Services.Publishers;
using BookingsApi.Swagger;
using Microsoft.Extensions.DependencyInjection;
using NSwag;
using NSwag.Generation.AspNetCore;
using NSwag.Generation.Processors.Security;
using ZymLabs.NSwag.FluentValidation;

namespace BookingsApi;

/// <summary>
/// Extension methods for configuring services
/// </summary>
public static class ConfigureServicesExtensions
{
    /// <summary>
    /// Add Api Versioning to the services
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddApiVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(opt =>
        {
            opt.DefaultApiVersion = new ApiVersion(1, 0);
            opt.AssumeDefaultVersionWhenUnspecified = true;
            opt.ReportApiVersions =
                true; // keep this true for backwards-compatibility with the old routes (i.e. the non versioned)
            opt.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader()
            );
        }).AddApiExplorer(setup =>
        {
            setup.GroupNameFormat = "'v'VVV"; // version format: 'v'[major][.minor][-status]
            setup.SubstituteApiVersionInUrl = true;
        });
        return services;
    }

    /// <summary>
    /// Add Swagger to the services
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddScoped(provider =>
        {
            var validationRules = provider.GetService<IEnumerable<FluentValidationRule>>();
            var loggerFactory = provider.GetService<ILoggerFactory>();

            return new FluentValidationSchemaProcessor(provider, validationRules, loggerFactory);
        });

        var apiVersionDescription = services.BuildServiceProvider().GetService<IApiVersionDescriptionProvider>();
        foreach (var groupName in apiVersionDescription.ApiVersionDescriptions.Select(x => x.GroupName))
        {
            services.AddOpenApiDocument((configure, servicesProvider) =>
            {
                ConfigureSwaggerForVersion(configure, groupName, new[] {groupName}, servicesProvider);
            });
        }

        // to build a single a client for all versions of the api, create one document with all the groups
        var groupNames = apiVersionDescription.ApiVersionDescriptions.Select(x => x.GroupName).ToArray();
        services.AddOpenApiDocument((configure, servicesProvider) =>
        {
            ConfigureSwaggerForVersion(configure, "all", groupNames, servicesProvider);
        });
        return services;
    }

    /// <summary>
    /// Add custom types to the services
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddCustomTypes(this IServiceCollection services)
    {
        services.AddMemoryCache();

        services.AddScoped<IQueryHandlerFactory, QueryHandlerFactory>();
        services.AddScoped<IQueryHandler, QueryHandler>();

        services.AddScoped<ICommandHandlerFactory, CommandHandlerFactory>();
        services.AddScoped<ICommandHandler, CommandHandler>();
        services.AddScoped<IHearingService, HearingService>();
        services.AddScoped<IRandomGenerator, RandomGenerator>();
        services.AddScoped<IHearingAllocationService, HearingAllocationService>();
        services.AddScoped<IRandomNumberGenerator, RandomNumberGenerator>();

        RegisterEventPublishers(services);

        services.AddScoped<IList<IPublishEvent>>(p => { 
            var list = p.GetServices<IPublishEvent>().ToList(); 
            list.AddRange(p.GetServices<IPublishMultidayEvent>().ToList());
            list.AddRange(p.GetServices<IPublishJudiciaryParticipantsEvent>().ToList());
            return list; });
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<IFirstdayOfMultidayBookingAsynchronousProcess, FirstdayOfMultidayHearingAsynchronousProcess>();
        services.AddScoped<ICreateConferenceAsynchronousProcess, CreateConferenceAsynchronousProcess>();
        services.AddScoped<IBookingAsynchronousProcess, SingledayHearingAsynchronousProcess>();
        services.AddScoped<IClonedBookingAsynchronousProcess, ClonedMultidaysAsynchronousProcess>();
        services.AddScoped<IEventPublisherFactory, EventPublisherFactory>();
        services.AddScoped<IHearingParticipantService, HearingParticipantService>();
        services.AddScoped<IParticipantAddedToHearingAsynchronousProcess, ParticipantAddedToHearingAsynchronousProcess>();
        services.AddScoped<INewJudiciaryAddedAsynchronousProcesses, NewJudiciaryAddedAsynchronousProcesses>();
        services.AddScoped<IEndpointService, EndpointService>();
        services.AddScoped<IJudiciaryParticipantService, JudiciaryParticipantService>();
        services.AddScoped<IUpdateHearingService, UpdateHearingService>();
        RegisterCommandHandlers(services);
        RegisterQueryHandlers(services);

        return services;
    }

    /// <summary>
    /// Add Json Options to the services
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <returns></returns>
    public static IServiceCollection AddJsonOptions(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
                options.JsonSerializerOptions.WriteIndented = true;
                options.JsonSerializerOptions.Converters.Add(new PascalCaseEnumConverterFactory());
            });

        return serviceCollection;
    }

    private static void RegisterEventPublishers(IServiceCollection serviceCollection)
    {
        var type = typeof(IPublishEvent);
        var derivedTypes = typeof(IPublishEvent).Assembly.GetTypes().Where(p => type.IsAssignableFrom(p));
        var derivedTypesExcludingInterfaces = derivedTypes.Where(x => !x.IsInterface);
        foreach (var eventPublisher in derivedTypesExcludingInterfaces)
        {
            var serviceType = eventPublisher.GetInterfaces()[0];
            serviceCollection.AddScoped(serviceType, eventPublisher);
        }
    }
    private static void RegisterCommandHandlers(IServiceCollection serviceCollection)
    {
        var commandHandlers = typeof(ICommand).Assembly.GetTypes().Where(t =>
            Array.Exists(t.GetInterfaces(), x =>
                x.IsGenericType &&
                x.GetGenericTypeDefinition() == typeof(ICommandHandler<>)));

        foreach (var queryHandler in commandHandlers)
        {
            var serviceType = queryHandler.GetInterfaces()[0];
            serviceCollection.AddScoped(serviceType, queryHandler);
        }
    }

    private static void RegisterQueryHandlers(IServiceCollection serviceCollection)
    {
        var queryHandlers = typeof(IQuery).Assembly.GetTypes().Where(t =>
            Array.Exists(t.GetInterfaces(), x =>
                x.IsGenericType &&
                x.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)));

        foreach (var queryHandler in queryHandlers)
        {
            var serviceType = queryHandler.GetInterfaces()[0];
            serviceCollection.AddScoped(serviceType, queryHandler);
        }
    }
        
    private static void ConfigureSwaggerForVersion(AspNetCoreOpenApiDocumentGeneratorSettings settings,
        string documentName, string[] apiGroupNames, IServiceProvider serviceProvider)
    {
        settings.DocumentName = documentName;
        settings.ApiGroupNames = apiGroupNames;
        settings.AddSecurity("JWT", [],
            new OpenApiSecurityScheme
            {
                Type = OpenApiSecuritySchemeType.ApiKey,
                Name = "Authorization",
                In = OpenApiSecurityApiKeyLocation.Header,
                Description = "Type into the textbox: Bearer {your JWT token}.",
                Scheme = "bearer"
            });
        settings.Title = "Bookings API";
        settings.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
        settings.OperationProcessors.Add(new AuthResponseOperationProcessor());

        var fluentValidationSchemaProcessor = serviceProvider.CreateScope().ServiceProvider
            .GetService<FluentValidationSchemaProcessor>();

        // Add the fluent validations schema processor
        settings.SchemaSettings.SchemaProcessors.Add(fluentValidationSchemaProcessor);
    }
}