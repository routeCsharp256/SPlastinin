using System;
using System.Net;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OzonEdu.MerchandiseService.Infrastructure.ApplicationService.Handlers;
using OzonEdu.MerchandiseService.Infrastructure.Filters;
using OzonEdu.MerchandiseService.Infrastructure.StartupFilters;
using Serilog;

namespace OzonEdu.MerchandiseService.Infrastructure.Extensions
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder ConfigureMicroserviceInfrastructure(this IHostBuilder builder)
        {
            builder.UseSerilog((context, configuration) => configuration
                .ReadFrom.Configuration(context.Configuration)
                .WriteTo.Console());
            
            builder.ConfigureServices((context, services) =>
            {
                services.AddSingleton<IStartupFilter, LoggingStartupFilter>();
                services.AddSingleton<IStartupFilter, TerminalStartupFilter>();

                services.AddSwagger();
                services.AddGrpc();

                services.AddControllers(options => options.Filters.Add<GlobalExceptionFilter>());

                services.AddMediatR(typeof(OrderChangeStatusDomainEventHandler).Assembly);

                services.AddDatabaseComponents(context.Configuration);
                services.AddRepositories();

                services.AddTracing(context.Configuration);
                
                services.AddKafkaServices(context.Configuration);

                services.AddStockApiGrpcServiceClient(context.Configuration);
            });

            return builder;
        }
    }
}