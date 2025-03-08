using Azure.Monitor.OpenTelemetry.AspNetCore;
using BookingsApi.DAL;
using BookingsApi.Domain.Configuration;
using BookingsApi.Health;
using BookingsApi.Infrastructure.Services.ServiceBusQueue;
using BookingsApi.Validations.Common;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace BookingsApi
{
    public class Startup(IConfiguration configuration)
    {
        private IConfiguration Configuration { get; } = configuration;
        public SettingsConfiguration SettingsConfiguration { get; private set; }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            
            var instrumentationKey = Configuration["ApplicationInsights:ConnectionString"];
            services.AddApiVersioning();
            services.AddControllers();
            
            services.AddOpenTelemetry()
                .ConfigureResource(r =>
                {
                    r.AddService("vh-bookings-api")
                        .AddTelemetrySdk()
                        .AddAttributes(new Dictionary<string, object>
                        { ["service.instance.id"] = Environment.MachineName });
                })
                .UseAzureMonitor(options => options.ConnectionString = instrumentationKey) 
                .WithMetrics()
                .WithTracing(tracerProvider =>
                {
                    tracerProvider
                        .AddAspNetCoreInstrumentation(options => options.RecordException = true)
                        .AddHttpClientInstrumentation();
                });
            var envName = Configuration["Services:BookingsApiResourceId"]; // any service url will do here since we only care about the env name
            services.AddSingleton<IFeatureToggles>(new FeatureToggles(Configuration["LaunchDarkly:SdkKey"], envName));
            services.AddValidatorsFromAssemblyContaining<RepresentativeValidation>(ServiceLifetime.Scoped, 
                filter =>
                {
                    var valid = !filter.ValidatorType.GetInterfaces().ToList().Contains(typeof(IRefDataInputValidator));
                    return valid;
                });
            
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

            services.AddVhHealthChecks(Configuration);
            services.AddMvc(options =>
            {
                // globally add a [ProducesResponseType] to all endpoints for a consistent swagger doc and API client.
                options.Filters.Add(new ProducesResponseTypeAttribute(typeof(string), 500));
            });
            services.AddCors();

            services.AddDbContextPool<BookingsDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("VhBookings"),
                    builder => builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(5), null)));
        }

        private void RegisterSettings(IServiceCollection services)
        {
            SettingsConfiguration = Configuration.Get<SettingsConfiguration>();
            services.Configure<AzureAdConfiguration>(options => Configuration.Bind("AzureAd", options));
            services.Configure<ServiceBusSettings>(options => Configuration.Bind("ServiceBusQueue", options));
            services.Configure<ServicesConfiguration>(options => Configuration.Bind("Services", options));
            services.Configure<SupplierConfiguration>(options => Configuration.Bind("SupplierConfiguration", options));
            services.Configure<AllocateHearingConfiguration>(options => Configuration.Bind("AllocateHearing", options));
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
            app.UseOpenApi();
            app.UseSwaggerUi(c =>
            {
                c.DocumentTitle = "Bookings API V1";
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else if (!SettingsConfiguration.DisableHttpsRedirection)
            {
                app.UseHsts();
                app.UseHttpsRedirection();
            }
            app.UseRouting();
            app.UseAuthorization(); 

            app.UseAuthentication();
            app.UseCors("CorsPolicy");

            app.UseMiddleware<RequestBodyLoggingMiddleware>();
            app.UseMiddleware<ExceptionMiddleware>();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                
                endpoints.MapHealthChecks("/health/liveness", new HealthCheckOptions()
                {
                    Predicate = check => check.Tags.Contains("self"),
                    ResponseWriter = HealthCheckResponseWriter
                });

                endpoints.MapHealthChecks("/health/startup", new HealthCheckOptions()
                {
                    Predicate = check => check.Tags.Contains("startup"),
                    ResponseWriter = HealthCheckResponseWriter
                });
                
                endpoints.MapHealthChecks("/health/readiness", new HealthCheckOptions()
                {
                    Predicate = check => check.Tags.Contains("readiness"),
                    ResponseWriter = HealthCheckResponseWriter
                });
            });
        }
        
        private async Task HealthCheckResponseWriter(HttpContext context, HealthReport report)
        {
            var result = JsonConvert.SerializeObject(new
            {
                status = report.Status.ToString(),
                details = report.Entries.Select(e => new
                {
                    key = e.Key, value = Enum.GetName(typeof(HealthStatus), e.Value.Status),
                    error = e.Value.Exception?.Message
                })
            });
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(result);
        }
    }
}