using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using HealthCheckPeek.HealthChecks;
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
                .AddCheck<WebsiteAvailabilityCheck1>("website_availability_check", tags: new[] { "website" })
                .AddDbContextCheck<MyDatabaseContext>("database_check", tags: new[] { "database" });


            services.AddDbContext<MyDatabaseContext>(options =>
            {
                options.UseSqlServer(@"");
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHealthChecks("/health",
               new HealthCheckOptions
               {
                   ResponseWriter = HealthCheckWriter.WriteResponses
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
                    ResponseWriter = HealthCheckWriter.WriteResponses
                });

                endpoints.MapHealthChecks("/health/websites", new HealthCheckOptions
                {
                    Predicate = (check) => check.Tags.Contains("websites"),
                    ResponseWriter = HealthCheckWriter.WriteResponses
                });

                endpoints.MapControllers();
            });
        }
    }
}
