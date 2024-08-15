using App.Client.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FluentUI.AspNetCore.Components;

namespace App.Client.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static void AddAppClientServices(this IServiceCollection services)
    {
        services.AddFluentUIComponents();
        services.AddSingleton<FetchDataService>();
        // Demo
        services.AddScoped<CacheStorageAccessor>();
        services.AddHttpClient<IStaticAssetService, HttpBasedStaticAssetService>();
    }
}