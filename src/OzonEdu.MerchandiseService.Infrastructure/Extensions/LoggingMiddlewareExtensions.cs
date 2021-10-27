using System;
using Microsoft.AspNetCore.Builder;
using OzonEdu.MerchandiseService.Infrastructure.Middlewares;

namespace OzonEdu.MerchandiseService.Infrastructure.Extensions
{
    public static class LoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            
            app.UseMiddleware<RequestLoggingMiddleware>();
            return app;
        }
        
        public static IApplicationBuilder UseResponseLogging(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            
            app.UseMiddleware<ResponseLoggingMiddleware>();
            return app;
        }
        
        public static IApplicationBuilder UseLogging(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            
            app.UseMiddleware<RequestLoggingMiddleware>();
            app.UseMiddleware<ResponseLoggingMiddleware>();
            return app;
        }
    }
}