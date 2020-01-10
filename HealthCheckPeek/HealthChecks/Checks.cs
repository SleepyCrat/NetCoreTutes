using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace HealthCheckPeek.HealthChecks
{
    public class WebsiteAvailabilityCheck1 : IHealthCheck
    {
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            using (var thePing = new Ping())
            {
                var pingResult = await thePing.SendPingAsync("");
                var description = "A ping of the website";
                var properties = new Dictionary<string, object>();
                properties.Add("RoundTripMS", pingResult.RoundtripTime);
                properties.Add("ActualIPAddress", pingResult.Address.ToString());
                properties.Add("ConfigurationWebAddress", "");
                if (pingResult.Status == IPStatus.Success)
                {
                    return HealthCheckResult.Healthy(description, properties);
                }

                return HealthCheckResult.Unhealthy(description, null, properties);
            }
        }
    }
}


public class HealthCheckWriter
{
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
}
