using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mime;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace HealthCheckPeek
{
    public class HealthCheckHelpers
    {
        public static async Task<HealthCheckResult> GenerateHealthCheckResultFromPIngRequest(string hostName)
        {
            using (var thePing = new Ping())
            {
                var pingResult = await thePing.SendPingAsync(hostName);
                var description = $"A ping of the {hostName} website";
                var healthCheckData = new Dictionary<string, object>();
                healthCheckData.Add("RoundTripMS", pingResult.RoundtripTime);
                healthCheckData.Add("ActualIPAddress", pingResult.Address.ToString());
                if (pingResult.Status == IPStatus.Success)
                {
                    return HealthCheckResult.Healthy(description, healthCheckData);
                }

                return HealthCheckResult.Unhealthy(description, null, healthCheckData);
            }
        }

        public static async Task<HealthCheckResult> RouteTimingHealthCheck(string routePath)
        {
            using (var client = new System.Net.WebClient())
            {
                var watch = new Stopwatch();
                watch.Start();
                var url = $"{BaseUrl}{routePath}";
                var result = await client.DownloadStringTaskAsync(url);
                watch.Stop();
                var milliseconds = watch.ElapsedMilliseconds;
                var healthCheckData = new Dictionary<string, object>();
                healthCheckData.Add("TimeInMS", milliseconds);
                if (milliseconds <= 1000)
                    return HealthCheckResult.Healthy($"call to  the route {routePath}", healthCheckData);
                else if (milliseconds >= 1001 && milliseconds <= 2000)
                    return HealthCheckResult.Degraded($"call to  the route {routePath}", null, healthCheckData);
                else
                    return HealthCheckResult.Unhealthy($"call to  the route {routePath}", null, healthCheckData);
            }
        }

        public static async Task WriteResponses(HttpContext context, HealthReport result)
        {
            var json = new JObject(
                            new JProperty("status", result.Status.ToString()),
                            new JProperty("results", new JObject(result.Entries.Select(pair =>
                            new JProperty(pair.Key, new JObject(
                                new JProperty("status", pair.Value.Status.ToString()),
                                new JProperty("description", pair.Value.Description),
                                new JProperty("data", new JObject(pair.Value.Data.Select(
                                    p => new JProperty(p.Key, p.Value))))))))));

            context.Response.ContentType = MediaTypeNames.Application.Json;
            await context.Response.WriteAsync(json.ToString(Formatting.Indented));
        }

        public static string BaseUrl { get; set; }
    }
}