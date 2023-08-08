using System.Net.Http;
using IdentityModel.Client;
using Sitecore.Abstractions;
using SitecoreMods.Foundation.Authorization.Models;

namespace SitecoreMods.Foundation.Authorization.RequestTypes
{
    public abstract class BaseOAuth2Request: BaseRequest
    {
        public BaseOAuth2Request(IHttpClientFactory httpClientFactory, BaseLog log) : base(httpClientFactory, log)
        {
        }

        protected abstract string ObtainToken();

        protected internal AuthorizationParameters GetAuthorizationParameters(string accessToken, string headerPrefix)
        {
            AuthorizationParameters authorizationParameters = new AuthorizationParameters();
            if (string.IsNullOrEmpty(accessToken))
                accessToken = this.ObtainToken();
            authorizationParameters.Headers.Add("Authorization", headerPrefix + " " + accessToken);
            return authorizationParameters;
        }

        protected internal string GetTokenEndpoint(System.Net.Http.HttpClient httpClient, string authority)
        {
            DiscoveryResponse result = httpClient.GetDiscoveryDocumentAsync(authority).Result;
            if (!result.IsError)
                return result.TokenEndpoint;
            Log.Error("Authorization: Dicovery request failed with error: " + result.Error, this);
            return null;
        }


    }
}