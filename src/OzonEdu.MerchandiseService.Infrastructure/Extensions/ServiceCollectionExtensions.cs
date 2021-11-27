using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Npgsql;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.EmployeeAggregate;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.MerchOrderAggregate;
using OzonEdu.MerchandiseService.Domain.SeedWork;
using OzonEdu.MerchandiseService.Infrastructure.Configuration;
using OzonEdu.MerchandiseService.Infrastructure.Interceptors;
using OzonEdu.MerchandiseService.Infrastructure.Repositories.Implementation;
using OzonEdu.MerchandiseService.Infrastructure.Repositories.Infrastructure.ChangeTracker;
using OzonEdu.MerchandiseService.Infrastructure.Repositories.Infrastructure.DbConnection;
using OzonEdu.MerchandiseService.Infrastructure.StartupFilters;

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
    }
}