using System;
using System.IO;
using System.Reflection;
using Grpc.Net.Client;
using Jaeger;
using Jaeger.Reporters;
using Jaeger.Samplers;
using Jaeger.Senders.Thrift;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Npgsql;
using OpenTracing;
using OpenTracing.Contrib.NetCore.Configuration;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.EmployeeAggregate;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.MerchOrderAggregate;
using OzonEdu.MerchandiseService.Domain.SeedWork;
using OzonEdu.MerchandiseService.Infrastructure.Configuration;
using OzonEdu.MerchandiseService.Infrastructure.Interceptors;
using OzonEdu.MerchandiseService.Infrastructure.MessageBroker;
using OzonEdu.MerchandiseService.Infrastructure.Repositories.Implementation;
using OzonEdu.MerchandiseService.Infrastructure.Repositories.Infrastructure.ChangeTracker;
using OzonEdu.MerchandiseService.Infrastructure.Repositories.Infrastructure.DbConnection;
using OzonEdu.MerchandiseService.Infrastructure.StartupFilters;
using OzonEdu.StockApi.Grpc;

namespace OzonEdu.MerchandiseService.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddSwagger(this IServiceCollection services)
        {
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
        }

        public static void AddGrpc(this IServiceCollection services)
        {
            services.AddGrpc(options =>
            {
                options.Interceptors.Add<ExceptionInterceptor>();
                options.Interceptors.Add<LoggingInterceptor>();
            });
        }

        public static void AddDatabaseComponents(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DatabaseConnectionOptions>(configuration.GetSection(nameof(DatabaseConnectionOptions)));
            services.AddScoped<IDbConnectionFactory<NpgsqlConnection>, NpgsqlConnectionFactory>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IChangeTracker, ChangeTracker>();
        }

        public static void AddRepositories(this IServiceCollection services)
        {
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IMerchOrderRepository, MerchOrderRepository>();
        }

        public static void AddTracing(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOpenTracing();
            
            // Adds the Jaeger Tracer.
            var jaegerConfigSection = configuration.GetSection(nameof(JaegerOptions));
            var jaegerHostname = jaegerConfigSection.GetValue<string>("Hostname");
            var jaegerPort = jaegerConfigSection.GetValue<int>("Port");
            services.AddSingleton<ITracer>(sp =>
            {
                var serviceName = sp.GetRequiredService<IWebHostEnvironment>().ApplicationName;
                var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
                var reporter = new RemoteReporter.Builder().WithLoggerFactory(loggerFactory)
                    .WithSender(new UdpSender(jaegerHostname, jaegerPort, 0))
                    .Build();
                var tracer = new Tracer.Builder(serviceName)
                    // The constant sampler reports every span.
                    .WithSampler(new ConstSampler(true))
                    // LoggingReporter prints every reported span to the logging framework.
                    .WithReporter(reporter)
                    .Build();
                return tracer;
            });

            services.Configure<HttpHandlerDiagnosticOptions>(options =>
                options.OperationNameResolver =
                    request => $"{request.Method.Method}: {request?.RequestUri?.AbsoluteUri}");
            
            services.AddSingleton<IStartupFilter, TracerStartupFilter>();
        }
        
        public static void AddKafkaServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IProducerBuilderWrapper, ProducerBuilderWrapper>();
        }
        
        public static void AddStockApiGrpcServiceClient(this IServiceCollection services, IConfiguration configuration)
        {
            var stockApiConfiguration = configuration.GetSection(nameof(StockApiGrpcServiceConfiguration));
            var connectionAddress = stockApiConfiguration?.GetValue<string>("ServerAddress");

            if (!string.IsNullOrWhiteSpace(connectionAddress))
            {
                services.AddScoped<StockApiGrpc.StockApiGrpcClient>(opt =>
                {
                    var channel = GrpcChannel.ForAddress(connectionAddress);
                    return new StockApiGrpc.StockApiGrpcClient(channel);
                });
            }
        }
    }
}