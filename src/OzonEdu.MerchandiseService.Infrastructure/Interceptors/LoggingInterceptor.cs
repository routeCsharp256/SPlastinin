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

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request,
            ServerCallContext context,
            UnaryServerMethod<TRequest, TResponse> continuation)
        {
            LogInformation(request, "Grpc request");

            var response = await continuation(request, context);

            LogInformation(response, "Grpc response");

            return response;
        }

        private void LogInformation<TValue>(TValue value, string message)
        {
            try
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    var valueJson = JsonSerializer.Serialize(value);
                    _logger.LogInformation("{Message} {Value}", message, valueJson);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not log {Message}", message);
            }
        }
    }
}