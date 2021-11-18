using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OzonEdu.MerchandiseService.Infrastructure.ApplicationService.Handlers;
using OzonEdu.MerchandiseService.Infrastructure.Filters;
using OzonEdu.MerchandiseService.Infrastructure.StartupFilters;

namespace OzonEdu.MerchandiseService.Infrastructure.Extensions
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder ConfigureMicroserviceInfrastructure(this IHostBuilder builder)
        {
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
            });

            return builder;
        }
    }
}