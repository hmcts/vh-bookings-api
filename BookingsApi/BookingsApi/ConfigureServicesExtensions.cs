using BookingsApi.Common.Security;
using BookingsApi.DAL.Services;
using BookingsApi.Swagger;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using NSwag;
using NSwag.Generation.Processors.Security;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Newtonsoft.Json.Converters;
using NSwag.Generation.AspNetCore;
using ZymLabs.NSwag.FluentValidation;


namespace BookingsApi
{
    public static class ConfigureServicesExtensions
    {
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
            }).AddVersionedApiExplorer(setup =>
            {
                setup.GroupNameFormat = "'v'VVV"; // version format: 'v'[major][.minor][-status]
                setup.SubstituteApiVersionInUrl = true;
            });
            return services;
        }

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

        private static void ConfigureSwaggerForVersion(AspNetCoreOpenApiDocumentGeneratorSettings settings,
            string documentName, string[] apiGroupNames, IServiceProvider serviceProvider)
        {
            settings.DocumentName = documentName;
            settings.ApiGroupNames = apiGroupNames;
            settings.AddSecurity("JWT", Enumerable.Empty<string>(),
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
            settings.SchemaProcessors.Add(fluentValidationSchemaProcessor);

        }

        public static IServiceCollection AddCustomTypes(this IServiceCollection services)
        {
            services.AddMemoryCache();

            services.AddSingleton<ITelemetryInitializer, BadRequestTelemetry>();

            services.AddScoped<ITokenProvider, AzureTokenProvider>();

            services.AddScoped<IQueryHandlerFactory, QueryHandlerFactory>();
            services.AddScoped<IQueryHandler, QueryHandler>();
            services.AddScoped<IFeatureFlagService, FeatureFlagService>();

            services.AddScoped<ICommandHandlerFactory, CommandHandlerFactory>();
            services.AddScoped<ICommandHandler, CommandHandler>();
            services.AddScoped<IHearingService, HearingService>();
            services.AddScoped<IRandomGenerator, RandomGenerator>();
            services.AddScoped<IVhLogger, VhLogger>();
            services.AddScoped<IHearingAllocationService, HearingAllocationService>();
            services.AddScoped<IRandomNumberGenerator, RandomNumberGenerator>();
            services.AddScoped<IBookingService, BookingService>();
            RegisterCommandHandlers(services);
            RegisterQueryHandlers(services);

            return services;
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

        public static IServiceCollection AddJsonOptions(this IServiceCollection serviceCollection)
        {
            var contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            };

            serviceCollection.AddMvc()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = contractResolver;
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                });

            return serviceCollection;
        }
    }
}