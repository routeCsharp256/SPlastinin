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
                StringBuilder sb = new();
                sb.Append("Response ")
                    .Append(context.Response?.StatusCode.ToString())
                    .Append(" for ")
                    .Append(context.Request?.Method)
                    .Append(" ")
                    .Append(context.Request?.Path.Value)
                    .Append(Environment.NewLine)
                    .Append(context.Response?.Headers.FormatHeaders());
                
                _logger.LogInformation(sb.ToString());
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not log request");
            }
        }
    }
}