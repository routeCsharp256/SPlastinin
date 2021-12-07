using System;
using Microsoft.AspNetCore.Builder;
using OzonEdu.MerchandiseService.Infrastructure.Middlewares;

namespace OzonEdu.MerchandiseService.Infrastructure.Extensions
{
    public static class TracerMiddlewareExtensions
    {
        public static IApplicationBuilder UseTracer(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            
            app.UseMiddleware<TracerMiddleware>();
            return app;
        }
    }
}