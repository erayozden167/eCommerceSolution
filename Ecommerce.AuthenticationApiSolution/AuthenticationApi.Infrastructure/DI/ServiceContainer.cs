using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Domain.Entities;
using AuthenticationApi.Infrastructure.Data;
using AuthenticationApi.Infrastructure.Repositories;
using eCommerce.SharedLibrary.DI;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationApi.Infrastructure.DI
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services,IConfiguration config)
        {
            SharedServiceContainer.AddSharedServices<AuthenticationDbContext>(services, config, config["MySerilog:FileName"]!);

            services.AddScoped<IUser, UserRepository>();

            return services;
        }
        public static IApplicationBuilder UserInfrastructurePolicy(this IApplicationBuilder app)
        {
            // Registered middlewares :
            // Global Exceptions
            // Listen Only Gateway
            SharedServiceContainer.UseSharedPolicies(app);
            return app;
        }
    }
}
