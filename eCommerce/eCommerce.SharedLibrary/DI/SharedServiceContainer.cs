using eCommerce.SharedLibrary.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace eCommerce.SharedLibrary.DI
{
    public static class SharedServiceContainer
    {
        public static IServiceCollection AddSharedServices<TContext>(this IServiceCollection services, IConfiguration configuration, string fileName) where TContext : DbContext
        {
            services.AddDbContext<TContext>(options => 
            options.UseSqlServer(configuration.GetConnectionString("eCommerceConnection"),
            sqlserverOption => sqlserverOption.EnableRetryOnFailure()));

            //serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Debug()
                .WriteTo.Console()
                .WriteTo.File(path: $"{fileName}-.text", restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {message:lj}{NewLine}{Exception}", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            //jwt
            JWTAuthScheme.AddJWTAuthScheme(services,configuration);
            return services;
        }
        public static IApplicationBuilder UseSharedPolicies(this IApplicationBuilder app)
        {
            //use global ex
            app.UseMiddleware<GlobalException>();

            // Kick Outsiders
            app.UseMiddleware<ListenOnlyApiGateway>();
            return app;
        }
    }
}
