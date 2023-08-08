using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;
using SitecoreMods.Foundation.Authorization.Factory;

namespace SitecoreMods.Foundation.Authorization
{
    public class ServicesConfigurator : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<RequestFactory>();
        }
    }
}