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
                StringBuilder sb = new();
                sb.Append("Request ")
                    .Append(context.Request?.Method)
                    .Append(" ")
                    .Append(context.Request?.Path.Value)
                    .Append(Environment.NewLine)
                    .Append(context.Request?.Headers.FormatHeaders());
                
                _logger.LogInformation(sb.ToString());
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not log request");
            }
        }
    }
}