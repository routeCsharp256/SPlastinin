using System;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace OzonEdu.MerchandiseService.Infrastructure.Extensions
{
    public static class WebHostBuilderExtensions
    {
        public static IWebHostBuilder ConfigurePorts(this IWebHostBuilder webBuilder)
        {
            var httpPortEnv = Environment.GetEnvironmentVariable("HTTP_PORT");
            if (!int.TryParse(httpPortEnv, out var httpPort))
            {
                httpPort = 5000;
            }

            var grpcPortEnv = Environment.GetEnvironmentVariable("GRPC_PORT");
            if (!int.TryParse(grpcPortEnv, out var grpcPort))
            {
                grpcPort = 5002;
            }
            
            if (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true")
            {
                httpPort = 80;
                grpcPort = 82;
            }
            
            webBuilder.ConfigureKestrel(
                options =>
                {
                    Listen(options, httpPort, HttpProtocols.Http1);
                    Listen(options, grpcPort, HttpProtocols.Http2);
                });

            return webBuilder;
        }
        
        private static void Listen(KestrelServerOptions kestrelServerOptions, int? port, HttpProtocols protocols)
        {
            if (port == null)
                return;

            var address = IPAddress.Any;

            kestrelServerOptions.Listen(address, port.Value, listenOptions => { listenOptions.Protocols = protocols; });
        }
    }
}