﻿using System;
using BookingsApi.Common;
using BookingsApi.Common.Security;
using BookingsApi.Common.Services;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.DAL.Services;
using BookingsApi.Services;
using BookingsApi.Swagger;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using NSwag;
using NSwag.Generation.Processors.Security;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NSwag.Generation.AspNetCore;

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
                opt.ReportApiVersions = true; // keep this true for backwards-compatibility with the old routes (i.e. the non versioned)
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
            var apiVersionDescription = services.BuildServiceProvider().GetService<IApiVersionDescriptionProvider>();
            foreach (var versionDescription in apiVersionDescription.ApiVersionDescriptions)
            {
                services.AddOpenApiDocument((configure) =>
                {
                    ConfigureSwaggerForVersion(configure, versionDescription.GroupName, new[] { versionDescription.GroupName });
                });
            }
            
            // to build a single a client for all versions of the api, create one document with all the groups
            var groupNames = apiVersionDescription.ApiVersionDescriptions.Select(x => x.GroupName).ToArray();
            services.AddOpenApiDocument((configure) =>
            {
                ConfigureSwaggerForVersion(configure, "all", groupNames);
            });
            return services;
        }

        private static void ConfigureSwaggerForVersion(AspNetCoreOpenApiDocumentGeneratorSettings configure,
             string documentName, string[] apiGroupNames)
        {
            configure.DocumentName = documentName;
            configure.ApiGroupNames = apiGroupNames;
            configure.AddSecurity("JWT", Enumerable.Empty<string>(),
                new OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.ApiKey,
                    Name = "Authorization",
                    In = OpenApiSecurityApiKeyLocation.Header,
                    Description = "Type into the textbox: Bearer {your JWT token}.",
                    Scheme = "bearer"
                });
            configure.Title = "Bookings API";
            configure.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
            configure.OperationProcessors.Add(new AuthResponseOperationProcessor());
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
            services.AddScoped<ILogger, Logger>();
            services.AddScoped<IHearingAllocationService, HearingAllocationService>();
            services.AddScoped<IRandomNumberGenerator, RandomNumberGenerator>();
            RegisterCommandHandlers(services);
            RegisterQueryHandlers(services);

            return services;
        }

        private static void RegisterCommandHandlers(IServiceCollection serviceCollection)
        {
            var commandHandlers = typeof(ICommand).Assembly.GetTypes().Where(t =>
                t.GetInterfaces().Any(x =>
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
                t.GetInterfaces().Any(x =>
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