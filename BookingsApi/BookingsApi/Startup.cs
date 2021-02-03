﻿using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BookingsApi.Common.Configuration;
using Bookings.Infrastructure.Services.IntegrationEvents;
using Bookings.Infrastructure.Services.ServiceBusQueue;
using BookingsApi.DAL;
using BookingsApi.Extensions;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Hosting;

namespace BookingsApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson();
            
            services.AddSingleton<ITelemetryInitializer>(new CloudRoleNameInitializer());
            
            services.AddApplicationInsightsTelemetry(Configuration["ApplicationInsights:InstrumentationKey"]);
            
            services.AddSwagger();
            services.AddCors(options => options.AddPolicy("CorsPolicy",
                builder =>
                {
                    builder
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .SetIsOriginAllowed((host) => true)
                        .AllowCredentials();
                }));
            
            services.AddJsonOptions();
            RegisterSettings(services);
            RegisterInfrastructureServices(services);

            services.AddCustomTypes();
            
            RegisterAuth(services);
            
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddCors();
            
            services.AddDbContextPool<BookingsDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("VhBookings")));
        }
        
        private void RegisterSettings(IServiceCollection services)
        {
            services.Configure<AzureAdConfiguration>(options => Configuration.Bind("AzureAd", options));
            services.Configure<ServiceBusSettings>(options => Configuration.Bind("ServiceBusQueue", options));
            services.Configure<ServicesConfiguration>(options => Configuration.Bind("Services", options));
            services.Configure<KinlyConfiguration>(options => Configuration.Bind("KinlyConfiguration", options));
        }

        private void RegisterAuth(IServiceCollection serviceCollection)
        {
            var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

            serviceCollection.AddMvc(options => { options.Filters.Add(new AuthorizeFilter(policy)); });

            var securitySettings = Configuration.GetSection("AzureAd").Get<AzureAdConfiguration>();
            var serviceSettings = Configuration.GetSection("Services").Get<ServicesConfiguration>();

            serviceCollection.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.Authority = $"{securitySettings.Authority}{securitySettings.TenantId}";
                options.TokenValidationParameters.ValidateLifetime = true;
                options.Audience = serviceSettings.BookingsApiResourceId;
                options.TokenValidationParameters.ClockSkew = TimeSpan.Zero;
                options.RequireHttpsMetadata = true;
            });

            serviceCollection.AddAuthorization();
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
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.RunLatestMigrations();
            
            app.UseOpenApi();
            app.UseSwaggerUi3(c =>
            {
                c.DocumentTitle = "Bookings API V1";
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
            app.UseRouting();
            app.UseAuthorization();
            
            app.UseAuthentication();
            app.UseCors("CorsPolicy");

            app.UseMiddleware<ExceptionMiddleware>();
            app.UseMiddleware<LogResponseBodyMiddleware>();

            app.UseEndpoints(endpoints => { endpoints.MapDefaultControllerRoute(); });            
        }
    }
}