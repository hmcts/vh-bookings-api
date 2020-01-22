using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.ApplicationInsights.Extensibility;
using Bookings.Common.Security;
using Bookings.Api.Contract.Requests;
using Bookings.API.Swagger;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using Bookings.Common;
using Bookings.DAL.Commands;
using Bookings.DAL.Commands.Core;
using Bookings.DAL.Queries.Core;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Bookings.API
{
    public static class ConfigureServicesExtensions
    {
        public static IServiceCollection AddSwagger(this IServiceCollection serviceCollection)
        {
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            var contractsXmlFile = $"{typeof(BookNewHearingRequest).Assembly.GetName().Name}.xml";
            var contractsXmlPath = Path.Combine(AppContext.BaseDirectory, contractsXmlFile);

            serviceCollection.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Bookings API", Version = "v1"});
                c.AddFluentValidationRules();
                c.IncludeXmlComments(xmlPath);
                c.IncludeXmlComments(contractsXmlPath);
                c.EnableAnnotations();

                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                c.OperationFilter<AuthResponsesOperationFilter>();
            });
            serviceCollection.AddSwaggerGenNewtonsoftSupport();

            return serviceCollection;
        }

        public static IServiceCollection AddCustomTypes(this IServiceCollection services)
        {
            services.AddMemoryCache();
            
            services.AddSingleton<ITelemetryInitializer, BadRequestTelemetry>();
            
            services.AddScoped<ITokenProvider, AzureTokenProvider>();

            services.AddScoped<IQueryHandlerFactory, QueryHandlerFactory>();
            services.AddScoped<IQueryHandler, QueryHandler>();
            
            services.AddScoped<ICommandHandlerFactory, CommandHandlerFactory>();
            services.AddScoped<ICommandHandler, CommandHandler>();
            services.AddScoped<IHearingService, HearingService>();
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