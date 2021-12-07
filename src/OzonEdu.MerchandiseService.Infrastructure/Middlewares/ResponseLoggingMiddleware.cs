using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OzonEdu.MerchandiseService.Infrastructure.Extensions;

namespace OzonEdu.MerchandiseService.Infrastructure.Middlewares
{
    public class ResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ResponseLoggingMiddleware> _logger;

        public ResponseLoggingMiddleware(RequestDelegate next, ILogger<ResponseLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public Task Invoke(HttpContext context)
        {
            if (!_logger.IsEnabled((LogLevel.Information)) || context.Request?.ContentType == "application/grpc")
            {
                return _next(context);
            }

            return InvokeInternal(context);
        }
        
        private async Task InvokeInternal(HttpContext context)
        {
            await _next(context);
            LogResponse(context);
        }
        
        private void LogResponse(HttpContext context)
        {
            try
            {
                _logger.LogInformation("Response {StatusCode} for {Method} {Path} {Headers}",
                    context.Response?.StatusCode.ToString(), context.Request?.Method,
                    context.Request?.Path.Value, context.Response?.Headers);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not log request");
            }
        }
    }
}