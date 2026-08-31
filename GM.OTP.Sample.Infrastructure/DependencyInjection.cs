using GM.Messaging;
using GM.OTP.Domain.Abstractions;
using GM.OTP.Options;
using GM.OTP.Sample.Infrastructure.Services;
using GM.OTP.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wolverine;

namespace GM.OTP.Sample.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Core OTP services (code generation, hashing, clock, <see cref="OtpManager"/>).
    /// Used by the API and by the inbox-processing worker.
    /// </summary>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        var otpOptions = new OtpOptions();
        configuration.GetSection("OtpOptions").Bind(otpOptions);
        services.AddSingleton(otpOptions);

        services.AddSingleton<ICodeGenerator, CodeGenerator>();
        services.AddSingleton<ICodeHasher, CodeHasher>();
        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();

        services.AddSingleton<OtpManager>(sp => new OtpManager(
            sp.GetRequiredService<ICodeGenerator>(),
            sp.GetRequiredService<ICodeHasher>(),
            sp.GetRequiredService<IDateTimeProvider>(),
            sp.GetRequiredService<OtpOptions>()));

        return services;
    }
    
    /// <summary>
    /// Same OTP services as <see cref="AddInfrastructure"/>, registered under a worker-specific
    /// entry point so the API and the inbox-processing worker can evolve independently later.
    /// </summary>
    public static IServiceCollection AddWorkerInfrastructure(
        this IServiceCollection services, IConfiguration configuration) =>
        services.AddInfrastructure(configuration);
    
    public static IServiceCollection AddConsumerWorkerInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddGMMessaging(configuration);
        return services;
    }
    
    public static IServiceCollection AddProducerWorkerInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddGMMessaging(configuration);
        return services;
    }
}
