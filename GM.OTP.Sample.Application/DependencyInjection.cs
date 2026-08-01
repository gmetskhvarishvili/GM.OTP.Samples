using System.Reflection;
using GM.API.Application.Behaviours;
using GM.Mediator;
using GM.Mediator.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace GM.OTP.Sample.Application;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddGMMediator(Assembly.GetExecutingAssembly());
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPerformanceBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));

        
    }
}