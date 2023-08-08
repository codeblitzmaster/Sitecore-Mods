using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.SecurityModel;
using SitecoreMods.Foundation.Authorization.Interfaces;
using SitecoreMods.Foundation.Authorization.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SitecoreMods.Foundation.Authorization.Fields;
using Sitecore.Diagnostics;
using System.Web.Util;
using Sitecore.ApplicationCenter.Applications;
using SitecoreMods.Foundation.Authorization.RequestTypes;
using Sitecore;
using SitecoreMods.Foundation.Authorization.Enums;
using SitecoreMods.Foundation.Authorization.Factory;

namespace SitecoreMods.Foundation.Authorization.Services
{
    public class ApiIntegrationService : IApiIntegrationService
    {
        private static readonly ConcurrentDictionary<string, RequestSettings> RequestSettingsCache = new ConcurrentDictionary<string, RequestSettings>();
        private static readonly ConcurrentDictionary<string, BaseRequest> RequestHandlersCache = new ConcurrentDictionary<string, BaseRequest>();
        private readonly RequestFactory _requestFactory;

        public ApiIntegrationService(RequestFactory requestFactory)
        {
            _requestFactory = requestFactory;
        }

        public Task<ResponseData> Fire(ID apiIntegrationItemId, Dictionary<string, object> data)
        {
            Item apiIntegrationItem;
            using (new SecurityDisabler())
                apiIntegrationItem = Sitecore.Context.Database.GetItem(apiIntegrationItemId);


            RequestSettings requestSettings = GetRequestSettings(apiIntegrationItem);
            BaseRequest requestSender = GetRequest(apiIntegrationItem, requestSettings);


            throw new NotImplementedException();
        }

        private BaseRequest GetRequest(Item apiIntegrationItem, RequestSettings requestSettings)
        {
            Item authItem = null;

            if (!string.IsNullOrEmpty(apiIntegrationItem[ApiIntegrationFieldIDs.Authorization]))
            {
                using (new SecurityDisabler())
                    authItem = apiIntegrationItem.Database.GetItem(apiIntegrationItem[ApiIntegrationFieldIDs.Authorization]);
                
                Assert.IsNotNull((object)authItem, $"Authorization item not found for Api Integration item {(object)apiIntegrationItem.Uri}");
            }

            string authCacheKey = GetAuthCacheKey(apiIntegrationItem, authItem);

            BaseRequest requestHandler;

            if (RequestHandlersCache.TryGetValue(authCacheKey, out requestHandler))
            {
                return requestHandler;
            }

            AuthorizationSettings authorizationSettings = new AuthorizationSettings();
            if (authItem != null)
            {
                authorizationSettings.AuthorizationType = ParseAuthType(authItem);
                foreach (TemplateFieldItem ownField in authItem.Template.OwnFields)
                    authorizationSettings.AuthProperties.Add(ownField.Name, authItem[ownField.ID]);
            }
            
            string key = RequestHandlersCache.Keys.SingleOrDefault(k => k.EndsWith(apiIntegrationItem.Statistics.Revision + "|", StringComparison.InvariantCultureIgnoreCase));


            if (key != null)
                RequestHandlersCache.TryRemove(key, out BaseRequest _);
            requestHandler = _requestFactory.GetRequestHandler(authorizationSettings);

            if (requestHandler != null)
                RequestHandlersCache.TryAdd(authCacheKey, requestHandler);
            return requestHandler;
        }

        private AuthorizationTypes ParseAuthType(Item authItem)
        {
            if (Enum.TryParse(authItem.TemplateName, true, out AuthorizationTypes result))
                return result;
            throw new NotSupportedException($"Authorization type {(object)authItem.TemplateName} defined by item {(object)authItem.Uri} is not supported.");
        }


        public Task<ResponseData> Fire(string apiIntegrationItemId, Dictionary<string, object> data)
        {
            return Fire(ID.Parse(apiIntegrationItemId), data);
        }

        private static RequestSettings GetRequestSettings(Item apiIntegrationItem)
        {
            string cacheKey = ApiIntegrationService.GetCacheKey(apiIntegrationItem);

            RequestSettings requestSettings;
            if (RequestSettingsCache.TryGetValue(cacheKey, out requestSettings))
            {
                return requestSettings;
            }
            requestSettings = new RequestSettings();

            requestSettings.Url = apiIntegrationItem[ApiIntegrationFieldIDs.Url];
            requestSettings.Method = apiIntegrationItem[ApiIntegrationFieldIDs.Method];
            requestSettings.Headers = new Dictionary<string, string>();

            var key = RequestSettingsCache.Keys.SingleOrDefault(k => k.StartsWith($"{(object)apiIntegrationItem.ID}|", StringComparison.InvariantCultureIgnoreCase));
            if (key != null)
                RequestSettingsCache.TryRemove(key, out RequestSettings _);
            RequestSettingsCache.TryAdd(cacheKey, requestSettings);
            return requestSettings;
        }



        /// <summary>
        /// Gets the cache key for the specified item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private static string GetCacheKey(Item item) => $"{(object)item.ID}|{(object)item.Statistics.Revision}";


        /// <summary>
        /// Gets the AuthItem cache key for the specified item.
        /// </summary>
        /// <param name="apiIntegrationItem"></param>
        /// <param name="authItem"></param>
        /// <returns></returns>
        private string GetAuthCacheKey(Item apiIntegrationItem, Item authItem) =>
            $"{(object)authItem?.ID}|{(object)authItem?.Statistics.Revision}|{(object)apiIntegrationItem.Statistics.Revision}";


    }
}
