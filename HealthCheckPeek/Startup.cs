using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.EntityFrameworkCore;

namespace HealthCheckPeek
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddHealthChecks()
                .AddAsyncCheck("google_ping_check", () => HealthCheckHelpers.GenerateHealthCheckResultFromPIngRequest("google.com"))
                .AddAsyncCheck("microsoft_ping_check", () => HealthCheckHelpers.GenerateHealthCheckResultFromPIngRequest("microsoft.com"))
                .AddAsyncCheck("yahoo_ping_check", () => HealthCheckHelpers.GenerateHealthCheckResultFromPIngRequest("yahoo.com"))
                .AddAsyncCheck("localhost_ping_check", () => HealthCheckHelpers.GenerateHealthCheckResultFromPIngRequest("localhost"))
                .AddAsyncCheck("forecast_time_check", () => HealthCheckHelpers.RouteTimingHealthCheck("/weatherforecast"))
                .AddAsyncCheck("forecast_time_check_slow", () => HealthCheckHelpers.RouteTimingHealthCheck("/weatherforecast/slow"))
                .AddDbContextCheck<MyDatabaseContext>("database_check", tags: new[] { "database" });
            services.AddHttpContextAccessor();

            services.AddDbContext<MyDatabaseContext>(options =>
            {
                options.UseSqlServer(@"Connection_string_here");
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.Use(async (context, next) =>
            {
                HealthCheckHelpers.BaseUrl = $"{ context.Request.Scheme}://{context.Request.Host}";
                await next();
            });

            app.UseHealthChecks("/health",
               new HealthCheckOptions
               {
                   ResponseWriter = HealthCheckHelpers.WriteResponses
               });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health/database", new HealthCheckOptions
                {
                    Predicate = (check) => check.Tags.Contains("database"),
                    ResponseWriter = HealthCheckHelpers.WriteResponses
                });

                endpoints.MapHealthChecks("/health/websites", new HealthCheckOptions
                {
                    Predicate = (check) => check.Tags.Contains("websites"),
                    ResponseWriter = HealthCheckHelpers.WriteResponses
                });

                endpoints.MapControllers();
            });
        }
    }
}
