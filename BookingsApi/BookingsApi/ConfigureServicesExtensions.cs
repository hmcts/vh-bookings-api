﻿using BookingsApi.Common;
using BookingsApi.Common.Security;
using BookingsApi.Common.Services;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.DAL.Services;
using BookingsApi.Services;
using BookingsApi.Swagger;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NSwag;
using NSwag.Generation.Processors.Security;
using System.Linq;

namespace BookingsApi
{
    public static class ConfigureServicesExtensions
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddOpenApiDocument((document, serviceProvider) =>
            {
                document.AddSecurity("JWT", Enumerable.Empty<string>(),
                    new OpenApiSecurityScheme
                    {
                        Type = OpenApiSecuritySchemeType.ApiKey,
                        Name = "Authorization",
                        In = OpenApiSecurityApiKeyLocation.Header,
                        Description = "Type into the textbox: Bearer {your JWT token}.",
                        Scheme = "bearer"
                    });
                document.Title = "Bookings API";
                document.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
                document.OperationProcessors.Add(new AuthResponseOperationProcessor());
            });
            return services;
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