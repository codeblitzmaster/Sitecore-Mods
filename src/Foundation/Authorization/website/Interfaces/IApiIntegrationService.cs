using Sitecore.Data;
using SitecoreMods.Foundation.Authorization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SitecoreMods.Foundation.Authorization.Interfaces
{
    /// <summary>
    /// Represents the service that is responsible for firing the api integration.
    /// </summary>
    internal interface IApiIntegrationService
    {
        /// <summary>
        /// Allows to invoke the api integration defined by the apiIntegrationItemId.
        /// </summary>
        /// <param name="apiIntegrationItemId"></param>
        /// <returns></returns>
        Task<ResponseData> Fire(ID apiIntegrationItemId, Dictionary<string, object> data);

        /// <summary>
        /// Allows to invoke the api integration defined by the apiIntegrationItemId.
        /// </summary>
        /// <param name="apiIntegrationItemId"></param>
        /// <returns></returns>
        Task<ResponseData> Fire(string apiIntegrationItemId, Dictionary<string, object> data);
    }
}
