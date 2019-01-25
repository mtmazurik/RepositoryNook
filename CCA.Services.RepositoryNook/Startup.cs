using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CCA.Services.RepositoryNook.Config;
using CCA.Services.RepositoryNook.Security;
using CCA.Services.RepositoryNook.Models;
using CCA.Services.RepositoryNook.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using CCA.Services.RepositoryNook.Service;
using System;

namespace CCA.Services.RepositoryNook
{
    public class Startup
    {
        private ILoggerFactory _loggerFactory;      // built in ASPNetCore logging factory
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
        private void OnShutdown()                                       // callback, applicationLifetime.ApplicationStopping triggers it
        {
           _logger.Log(LogLevel.Information, "RepositoryNook service stopped.");
        }

        public void ConfigureServices(IServiceCollection services)      // add services to the ASPNETCore App. This gets called by the WebHost runtime. 
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .WithMethods("Get", "Post", "Put")
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            
            services.AddAuthentication(options =>                               // using a free Auth0.com account for API Endpoint security (authentication/bearer token)
            {
                   options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                   options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
               }).AddJwtBearer(options =>
               {
                   options.Authority = $"https://{_configuration["Auth0:Domain"]}/";
                   options.Audience = _configuration["Auth0:ApiIdentifier"];
               }
            );


            services.AddApplicationInsightsTelemetry(_configuration);           // Azure Application Insights statistical data turned on (telemetry)
            IServiceProvider serviceProvider = services.BuildServiceProvider();
            _loggerFactory.AddApplicationInsights(serviceProvider, LogLevel.Information);   // ASPNetCore logger instance causes everyting Information (and above) to be sent to AppInsights

            services.AddMvc(options =>                                  
            {
                options.Filters.Add(new AllowAnonymousFilter(_logger));         // Controller filter. Lets [Anonymous] attribute on REST method (no security for that REST call)
            }).AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Include;
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            });


            // services.AddSingleton<IHostedService, TaskManager>();            // Tasks/ background task manager  (for multi-thread processing (optional) can turn on here)


            services.AddSwaggerGen(options =>                                   // swagger - autodocument setup
            {
                options.DescribeAllEnumsAsStrings();
                options.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info
                {
                    Title = "RepositoryNook Service",
                    Version = "v1",
                    Description = "RESTful API, micro service called 'RepositoryNook'",
                    TermsOfService = "(C) 2018 Cloud Computing Associates (CCA)  All Rights Reserved."
                });
            });

            services.AddTransient<IResponse, Response>();                       // Dependency injection (DI).  ASPNETCore's built-in
            services.AddTransient<HttpClient>();
            services.AddTransient<IJsonConfiguration, JsonConfiguration>();
            services.AddTransient<IWorker, Worker>();
            services.AddTransient<IRepositoryNookService, RepositoryNookService>();

        }
        public void Configure(IApplicationBuilder app, IApplicationLifetime applicationLifetime, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            
            app.UseStaticFiles();                                               // swagger related
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "RepositoryNook Service");
            });

            app.UseAuthentication();                                            // JWT Auth. ASPNETCore built-in functionality

            app.UseCors("CorsPolicy");                                          // Cross-Origin Resource Sharing

            app.UseMvc();

            _logger.Log(LogLevel.Information,"RepositoryNook service started.");       // log start

            applicationLifetime.ApplicationStopping.Register( OnShutdown );     // hook callback for on-shutdown event
        }
    }
}
