using System;
using System.IO;
using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.EmployeeAggregate;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.MerchOrderAggregate;
using OzonEdu.MerchandiseService.Infrastructure.DomainService.Handlers;
using OzonEdu.MerchandiseService.Infrastructure.Filters;
using OzonEdu.MerchandiseService.Infrastructure.Interceptors;
using OzonEdu.MerchandiseService.Infrastructure.Repositories.Mocks;
using OzonEdu.MerchandiseService.Infrastructure.StartupFilters;

namespace OzonEdu.MerchandiseService.Infrastructure.Extensions
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder AddInfrastructure(this IHostBuilder builder)
        {
            builder.ConfigureServices((_,services) =>
            {
                services.AddSingleton<IStartupFilter, LoggingStartupFilter>();
                services.AddSingleton<IStartupFilter, TerminalStartupFilter>();
                services.AddSingleton<IStartupFilter, SwaggerStartupFilter>();
                services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo {Title = "OzonEdu.MerchandiseService", Version = "v1"});
                    options.CustomSchemaIds(x => x.FullName);

                    var assemblyName = Assembly.GetEntryAssembly()?.GetName().Name;
                    if (assemblyName != null)
                    {
                        var xmlFileName = assemblyName + ".xml";
                        var xmlFilePath = Path.Combine(AppContext.BaseDirectory, xmlFileName);
                        options.IncludeXmlComments(xmlFilePath);
                    }
                });
                services.AddGrpc(options =>
                {
                    options.Interceptors.Add<ExceptionInterceptor>();
                    options.Interceptors.Add<LoggingInterceptor>();
                });
                services.AddControllers(options => options.Filters.Add<GlobalExceptionFilter>());
                
                services.AddMediatR(typeof(OrderChangeStatusDomainEventHandler).Assembly);
                
                services.AddSingleton<IEmployeeRepository, MockEmployeeRepository>();
                services.AddSingleton<IMerchOrderRepository, MockMerchOrderRepository>();
            });

            return builder;   
        }
    }
}