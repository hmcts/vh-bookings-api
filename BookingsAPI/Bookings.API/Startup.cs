using System;
using System.Collections.Generic;
using Bookings.API.Authorization;
using Bookings.API.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Bookings.Common.Configuration;
using Bookings.DAL;
using Bookings.Infrastructure.Services.IntegrationEvents;
using Bookings.Infrastructure.Services.ServiceBusQueue;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Authentication;

namespace Bookings.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }
        private AzureAdConfiguration AzureAdSettings { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ITelemetryInitializer>(new CloudRoleNameInitializer());
            
            services.AddSwagger();
            services.AddJsonOptions();
            RegisterSettings(services);
            RegisterInfrastructureServices(services);

            services.AddCustomTypes();
            
            RegisterAuth(services);
            
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddCors();
            
            services.AddDbContextPool<BookingsDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("VhBookings")));
        }
        
        private void RegisterSettings(IServiceCollection services)
        {
            AzureAdSettings = Configuration.GetSection("AzureAd").Get<AzureAdConfiguration>();
            services.Configure<AzureAdConfiguration>(options => Configuration.Bind("AzureAd", options));
            services.Configure<ServiceBusSettings>(options => Configuration.Bind("ServiceBusQueue", options));
        }
        
        private void RegisterAuth(IServiceCollection services)
        {
            services.AddMvc(options =>
            {
                options.Filters.Add(new AuthorizeFilter(Policies.Default));
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddAuthorization(options =>
            {
                options.AddPolicy(Policies.Default, policy =>
                {
                    policy.RequireAuthenticatedUser();
                });
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Authority = $"{AzureAdSettings.Authority}/{AzureAdSettings.TenantId}/v2.0";
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidAudiences = new List<string>
                    {
                        AzureAdSettings.AppRegistrationId,
                        AzureAdSettings.ClientId
                    }
                };
            });

            services.AddSingleton<IClaimsTransformation, AzureAdScopeClaimTransformation>();
            services.AddSingleton<IAuthorizationHandler, PermissionRequirementHandler>();
        }
        private void RegisterInfrastructureServices(IServiceCollection services)
        {
            bool.TryParse(Configuration["UseServiceBusFake"], out var useFakeClient);
            if (useFakeClient)
            {
                services.AddSingleton<IServiceBusQueueClient, ServiceBusQueueClientFake>();
            }
            else
            {
                services.AddScoped<IServiceBusQueueClient, ServiceBusQueueClient>();
            }

            services.AddScoped<IEventPublisher, EventPublisher>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.RunLatestMigrations();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                const string url = "/swagger/v1/swagger.json";
                c.SwaggerEndpoint(url, "Bookings API V1");
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseAuthentication();
            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowCredentials()
                .AllowAnyHeader());

            app.UseMiddleware<LogResponseBodyMiddleware>();
            app.UseMiddleware<ExceptionMiddleware>();

            app.UseMvc();
        }
    }
}