using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace OzonEdu.MerchandiseService.Infrastructure.Middlewares
{
    public class VersionMiddleware
    {
        public VersionMiddleware(RequestDelegate next)
        {
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string version = Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "no version";
            string serviceName = Assembly.GetEntryAssembly()?.GetName().Name ?? "no name";
            var answer = new Dictionary<string, string>() {{"version", version}, {"serviceName", serviceName}};
            await context.Response.WriteAsync(JsonSerializer.Serialize(answer));
        }
    }
}