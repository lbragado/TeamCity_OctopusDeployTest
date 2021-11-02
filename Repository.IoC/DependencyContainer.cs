using Entities.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repository.EFCore.DataContext;
using Repository.EFCore.Repositories;
using Repository.EFCore.UnitOfWork;

namespace Repository.IoC
{
    public static class DependencyContainer
    {
        /// <summary>
        /// Permitirá inyectar el reposotorio en Startup.ConfigureServices
        /// </summary>
        /// <param name="services">Extensión para IserviceCollection</param>
        /// <param name="configuration">Nos permitirá leer la cadena de conexión</param>
        /// <returns>Servicio</returns>
        public static IServiceCollection AddRepositories(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<RepositoryContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("RepositoryDB")));

            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<ICurrentbalanceRepository, CurrentbalanceRepository>();
            services.AddScoped<IOperationRepository, OperationRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            
            return services;
        }
    }
}
