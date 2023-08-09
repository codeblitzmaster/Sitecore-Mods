using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;
using SitecoreMods.Foundation.Authorization.Factory;
using SitecoreMods.Foundation.Authorization.Interfaces;
using SitecoreMods.Foundation.Authorization.Services;

namespace SitecoreMods.Foundation.Authorization
{
    public class ServicesConfigurator : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddHttpClient();
            serviceCollection.AddSingleton<RequestFactory>();
            serviceCollection.AddSingleton<IApiIntegrationService, ApiIntegrationService>();
        }
    }
}