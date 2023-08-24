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
using Sitecore.Abstractions;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Formatting = System.Xml.Formatting;
using Sitecore.Configuration.KnownSettings;
using SitecoreMods.Foundation.Authorization.Extensions;
using AuthorizationSettings = SitecoreMods.Foundation.Authorization.Models.AuthorizationSettings;
using System.Collections.Specialized;

namespace SitecoreMods.Foundation.Authorization.Services
{
    public class ApiIntegrationService : IApiIntegrationService
    {
        private static readonly ConcurrentDictionary<string, RequestSettings> RequestSettingsCache = new ConcurrentDictionary<string, RequestSettings>();
        private static readonly ConcurrentDictionary<string, BaseRequest> RequestHandlersCache = new ConcurrentDictionary<string, BaseRequest>();
        private readonly RequestFactory _requestFactory;
        private readonly BaseLog _baseLog;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public ApiIntegrationService(RequestFactory requestFactory, BaseLog baseLog)
        {
            _requestFactory = requestFactory;
            _baseLog = baseLog;
        }

        public async Task<ResponseData> FireAsync(ID apiIntegrationItemId, Dictionary<string, object> data)
        {
            Item apiIntegrationItem;
            using (new SecurityDisabler())
                apiIntegrationItem = Sitecore.Context.Database.GetItem(apiIntegrationItemId);

            try
            {

                RequestSettings requestSettings = GetRequestSettings(apiIntegrationItem);
                BaseRequest requestSender = GetRequest(apiIntegrationItem, requestSettings);

                if (data != null)
                {
                    var content = data.ExpandFlattenedObjectToJsonString();
                    return await requestSender.SendRequestAsync(requestSettings, content, _cancellationTokenSource.Token);
                }
                var responseData = await requestSender.SendRequestAsync(requestSettings, null, _cancellationTokenSource.Token);
                if (_baseLog.DebugEnabled(this))
                {
                    _baseLog.Debug($"::ApiIntegration:: Fired successful integration for {apiIntegrationItem.ID}/{apiIntegrationItem.Name}", this);
                }
                return responseData;
            }
            catch (Exception ex)
            {
                _baseLog.Error($"::ApiIntegration:: Error occured while executing the integration for {apiIntegrationItem.ID}/{apiIntegrationItem.Name}", ex, this);
                throw;
            }
        }

        private BaseRequest GetRequest(Item apiIntegrationItem, RequestSettings requestSettings)
        {
            Item authItem = null;

            if (!string.IsNullOrEmpty(apiIntegrationItem.Fields[ApiIntegrationFieldIDs.Authorization].Value))
            {
                using (new SecurityDisabler())
                    authItem = apiIntegrationItem.Database.GetItem(apiIntegrationItem.Fields[ApiIntegrationFieldIDs.Authorization].Value);
                
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
            throw new NotSupportedException($"::ApiIntegration:: Authorization type {(object)authItem.TemplateName} defined by item {(object)authItem.Uri} is not supported.");
        }


        public async Task<ResponseData> FireAsync(string apiIntegrationItemId, Dictionary<string, object> data)
        {
            return await FireAsync(ID.Parse(apiIntegrationItemId), data);
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
            var customHeaders = apiIntegrationItem[ApiIntegrationFieldIDs.Headers];
            requestSettings.Headers = new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(customHeaders)) {
                NameValueCollection customHeadersCollection = Sitecore.Web.WebUtil.ParseUrlParameters(customHeaders);
                foreach (var customHeaderKey in customHeadersCollection.AllKeys)
                {
                    requestSettings.Headers.Add(customHeaderKey, customHeadersCollection[customHeaderKey]);
                }
            }
            requestSettings.ContentType = apiIntegrationItem[ApiIntegrationFieldIDs.ContentType];
            requestSettings.SetContentTypeHeader();

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

        private static Dictionary<string, object> ExpandFlattenedObject(Dictionary<string, object> flattenedObject)
        {
            Dictionary<string, object> expandedObject = new Dictionary<string, object>();

            foreach (var kvp in flattenedObject)
            {
                string[] keys = kvp.Key.Split('.');
                string[] arrayIndexParts = keys[keys.Length - 1].Split('[');

                // Initialize nested dictionaries if needed
                Dictionary<string, object> currentDict = expandedObject;
                for (int i = 0; i < keys.Length - 1; i++)
                {
                    if (!currentDict.ContainsKey(keys[i]))
                    {
                        currentDict[keys[i]] = new Dictionary<string, object>();
                    }
                    currentDict = (Dictionary<string, object>)currentDict[keys[i]];
                }

                // Handle array index if present
                if (arrayIndexParts.Length > 1)
                {
                    string arrayKey = arrayIndexParts[0];
                    int arrayIndex = int.Parse(arrayIndexParts[1].TrimEnd(']'));

                    if (!currentDict.ContainsKey(arrayKey))
                    {
                        currentDict[arrayKey] = new List<object>();
                    }

                    List<object> currentList = (List<object>)currentDict[arrayKey];
                    while (currentList.Count <= arrayIndex)
                    {
                        currentList.Add(null);
                    }
                    currentList[arrayIndex] = kvp.Value;
                }
                else
                {
                    currentDict[keys[keys.Length - 1]] = kvp.Value;
                }
            }

            return expandedObject;
        }
    }
}
