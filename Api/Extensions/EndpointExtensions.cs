using System.Reflection;
using Api.Abstractions;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Api.Extensions;

public static class EndpointExtensions
{
    public static IServiceCollection AddEndpoints(this IServiceCollection services, Assembly assembly)
    {
        var serviceDescriptors = assembly
            .GetExportedTypes()
            .Where(type => type is { IsClass: true, IsAbstract: false } && typeof(IEndpoint).IsAssignableFrom(type))
            .Select(type => ServiceDescriptor.Transient(typeof(IEndpoint), type))
            .ToArray();

        services.TryAddEnumerable(serviceDescriptors);

        return services;
    }

    public static IApplicationBuilder MapEndpoints(this WebApplication app)
    {
        var endpoints = app.Services.CreateScope().ServiceProvider.GetServices<IEndpoint>();

        var enumerable = endpoints.ToList();
        if (!enumerable.Any())
        {
            var type = typeof(IEndpoint);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract);

            foreach (var t in types)
            {
                var instance = (IEndpoint)Activator.CreateInstance(t)!;
                instance.MapEndpoint(app);
            }
        }
        else
        {
            foreach (var endpoint in enumerable)
            {
                endpoint.MapEndpoint(app);
            }
        }

        return app;
    }
}