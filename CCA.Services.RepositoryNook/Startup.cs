using System;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.Extensions.Logging;
using CCA.Services.RepositoryNook.Config;
using CCA.Services.RepositoryNook.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using CCA.Services.RepositoryNook.Services;
using CCA.Services.RepositoryNook.HelperClasses;

namespace CCA.Services.RepositoryNook
{
    public class Startup
    {
        private ILoggerFactory _loggerFactory;                                              // leverage built in ASPNetCore logging 
        private ILogger<Startup> _logger;
        private IConfigurationRoot _configuration { get; }


        public Startup(Microsoft.AspNetCore.Hosting.IHostingEnvironment env, ILogger<Startup> logger, ILoggerFactory loggerFactory)       // ctor
        {
            var builder = new ConfigurationBuilder()        
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            _configuration = builder.Build();
            _logger = logger;
            _loggerFactory = loggerFactory;
        }
        private void OnShutdown()                                                           // shutdown; leverages applicationLifetime.ApplicationStopping which triggers it
        {
           _logger.Log(LogLevel.Information, "RepositoryNook service stopped.");
        }

        public void ConfigureServices(IServiceCollection services)                          // called by the WebHost runtime 
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .WithMethods("Get", "Post", "Put")
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            
            services.AddAuthentication(options =>                                           // can use free Auth0.com account for API Endpoint (AUTH) security; authentication/bearer token
            {                                                                               // or default: anonymous calls with attributes in the controller [Anonymous]
                   options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                   options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
               }).AddJwtBearer(options =>
               {
                   options.Authority = $"https://{_configuration["Auth0:Domain"]}/";
                   options.Audience = _configuration["Auth0:ApiIdentifier"];
               }
            );


            services.AddApplicationInsightsTelemetry(_configuration);                       // Azure Application Insights statistical data turned on (telemetry)

            IServiceProvider serviceProvider = services.BuildServiceProvider();
            _loggerFactory.AddApplicationInsights(serviceProvider, LogLevel.Information);   // ASPNetCore logger instance causes everyting Information (and above) to be sent to AppInsights

            services.AddMvc(options =>                                  
            {
                options.Filters.Add(new AllowAnonymousFilter(_logger));                     // shim: implented for [Anonymous] attribute on REST method (no security for that REST call)
            }).AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Include;
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            });

            services.AddSwaggerGen(options =>                                               // swagger - autodocument setup
            {
                options.DescribeAllEnumsAsStrings();
                options.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info
                {
                    Title = "RepositoryNook Service",
                    Version = "v1",
                    Description = "API for service called 'RepositoryNook' stores in MongoDB Atlas (hosted)",
                    TermsOfService = "(C) 2018 Cloud Computing Associates (CCA)  All Rights Reserved."
                });
            });

            services.AddTransient<IResponse, Response>();                                   // leverage Dotnet core dependency injection (DI)
            services.AddTransient<HttpClient>();
            services.AddTransient<IJsonConfiguration, JsonConfiguration>();
            services.AddTransient<IRepositoryService, RepositoryService>();
            services.AddTransient<IAdminService, AdminService>();
        }
        public void Configure(IApplicationBuilder app, IApplicationLifetime applicationLifetime, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            
            app.UseStaticFiles();                                                           // swagger related things
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "RepositoryNook Service");
            });

            app.UseAuthentication();                                                        // JWT Auth. ASPNETCore built-in functionality

            app.UseCors("CorsPolicy");                                                      // CORS; Cross-Origin Resource Sharing

            app.UseMvc( routes =>
            {
                routes.MapRoute("admin", "{controller=AdminController}/{action=Index}");
                routes.MapRoute("default", "{controller=RepositoryNookController}/{action=Index}");
            });

            _logger.Log(LogLevel.Information,"RepositoryNook service started.");            // log start of service

            applicationLifetime.ApplicationStopping.Register( OnShutdown );                 // hook callback for on-shutdown event
        }
    }
}
