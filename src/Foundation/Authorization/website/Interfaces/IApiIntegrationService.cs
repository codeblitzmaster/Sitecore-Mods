using Sitecore.Data;
using SitecoreMods.Foundation.Authorization.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SitecoreMods.Foundation.Authorization.Interfaces
{
    /// <summary>
    /// Represents the service that is responsible for firing the api integration.
    /// </summary>
    public interface IApiIntegrationService
    {
        /// <summary>
        /// Allows to invoke the api integration defined by the apiIntegrationItemId.
        /// </summary>
        /// <param name="apiIntegrationItemId"></param>
        /// <returns></returns>
        Task<ResponseData> FireAsync(ID apiIntegrationItemId, Dictionary<string, object> data);

        /// <summary>
        /// Allows to invoke the api integration defined by the apiIntegrationItemId.
        /// </summary>
        /// <param name="apiIntegrationItemId"></param>
        /// <returns></returns>
        Task<ResponseData> FireAsync(string apiIntegrationItemId, Dictionary<string, object> data);
    }
}
