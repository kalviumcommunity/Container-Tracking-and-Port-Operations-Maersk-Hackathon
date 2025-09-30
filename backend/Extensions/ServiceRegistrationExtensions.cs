using Backend.Repositories;
using Backend.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.Extensions
{
    public static class ServiceRegistrationExtensions
    {
        /// <summary>
        /// Registers all repositories with the dependency injection container
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection RegisterRepositories(this IServiceCollection services)
        {
            // Register generic repository
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            
            // Register specific repositories
            services.AddScoped<IContainerRepository, ContainerRepository>();
            services.AddScoped<IShipRepository, ShipRepository>();
            services.AddScoped<IPortRepository, PortRepository>();
            services.AddScoped<IBerthRepository, BerthRepository>();
            services.AddScoped<IBerthAssignmentRepository, BerthAssignmentRepository>();
            services.AddScoped<IShipContainerRepository, ShipContainerRepository>();
            
            return services;
        }
        
        /// <summary>
        /// Registers all services with the dependency injection container
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            // Register services
            services.AddScoped<IContainerService, ContainerService>();
            services.AddScoped<IShipService, ShipService>();
            services.AddScoped<IPortService, PortService>();
            services.AddScoped<IBerthService, BerthService>();
            services.AddScoped<IBerthAssignmentService, BerthAssignmentService>();
            services.AddScoped<IShipContainerService, ShipContainerService>();
            
            return services;
        }
    }
}