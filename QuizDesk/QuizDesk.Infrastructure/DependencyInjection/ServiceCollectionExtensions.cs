using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuizDesk.Application.Abstractions;
using QuizDesk.Application.Common.Settings;
using QuizDesk.Application.Interfaces.Auth;
using QuizDesk.Application.Interfaces.OAuth;
using QuizDesk.Application.Services.Auth;
using QuizDesk.Application.Services.OAuth;
using QuizDesk.Domain.Interfaces;
using QuizDesk.Infrastructure.Persistance.Repositories;
using QuizDesk.Infrastructure.Persistance.UnitOfWork;
using QuizDesk.Infrastructure.Services.Identity;
using QuizDesk.Infrastructure.Services.OAuth;
using QuizDesk.Infrastructure.Services.Redis;
using QuizDesk.Infrastructure.Services.Security;
using StackExchange.Redis;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(
    this IServiceCollection services,
    IConfiguration configuration)
    {
        var postgresConnection = configuration.GetConnectionString("PostgreSQL")
            ?? throw new InvalidOperationException("PostgreSQL connection string is missing!");

        services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(
                    postgresConnection,
                    npgsql => npgsql.EnableRetryOnFailure()
                ));

        var redisConnection = configuration.GetConnectionString("Redis")
            ?? throw new InvalidOperationException("Redis connection string is missing.");

        services.AddSingleton<IConnectionMultiplexer>(_ =>
        {
            var config = ConfigurationOptions.Parse(redisConnection);
            config.AbortOnConnectFail = false;
            config.ReconnectRetryPolicy = new LinearRetry(5000);
            return ConnectionMultiplexer.Connect(config);
        });

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnection;
            options.InstanceName = "QuizDesk_";
        });
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));


        //Register Application & Infrastructure Services
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IOAuthRepository, OAuthRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenService, TokenService>();

        services.AddScoped<IOAuthService, OAuthService>();
        services.AddScoped<IOAuthProviderService, GoogleOAuthProviderService>();
        services.AddScoped<ISessionStore, RedisSessionStore>();

        return services;
    }
}