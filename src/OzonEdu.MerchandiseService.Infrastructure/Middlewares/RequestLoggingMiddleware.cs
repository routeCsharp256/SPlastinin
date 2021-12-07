using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OzonEdu.MerchandiseService.Infrastructure.Extensions;

namespace OzonEdu.MerchandiseService.Infrastructure.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task Invoke(HttpContext context)
        {
            if (!_logger.IsEnabled(LogLevel.Information) || context.Request?.ContentType == "application/grpc")
            {
                return _next(context);
            }
            
            LogRequest(context);
            return _next(context);
        }
        
        private void LogRequest(HttpContext context)
        {
            try
            {
                _logger.LogInformation("Request {RequestMethod} {Path} {Headers}", 
                    context.Request?.Method, context.Request?.Path.Value, context.Request?.Headers);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not log request");
            }
        }
    }
}