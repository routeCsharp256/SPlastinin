using System;
using System.Text.Json;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;

namespace OzonEdu.MerchandiseService.Infrastructure.Interceptors
{
    public class LoggingInterceptor : Interceptor
    {
        private readonly ILogger<LoggingInterceptor> _logger;

        public LoggingInterceptor(ILogger<LoggingInterceptor> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context,
            UnaryServerMethod<TRequest, TResponse> continuation)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                var requestJson = JsonSerializer.Serialize(request);
                _logger.LogInformation(requestJson);    
            }

            var response = base.UnaryServerHandler(request, context, continuation);

            if (_logger.IsEnabled(LogLevel.Information))
            {
                var responseJson = JsonSerializer.Serialize(response);
                _logger.LogInformation(responseJson);
            }

            return response;
        }
    }
}