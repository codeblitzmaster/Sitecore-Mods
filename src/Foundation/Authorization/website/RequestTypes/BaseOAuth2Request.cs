using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel.Client;
using Sitecore.Abstractions;
using SitecoreMods.Foundation.Authorization.Models;

namespace SitecoreMods.Foundation.Authorization.RequestTypes
{
    public abstract class BaseOAuth2Request : BaseRequest
    {
        public BaseOAuth2Request(IHttpClientFactory httpClientFactory, BaseLog log) : base(httpClientFactory, log)
        {
        }

        public override async Task<ResponseData> SendAsync(string content, RequestSettings requestSettings,
            AuthorizationParameters authorizationParameters, CancellationToken token)
        {
            ResponseData errorWhenEmptyToken = GetResponseDataWithErrorWhenEmptyToken(authorizationParameters);
            
            if (errorWhenEmptyToken != null)
                return errorWhenEmptyToken;


            ResponseData responseData = await base.SendAsync(content, requestSettings, authorizationParameters, token).ConfigureAwait(false);
            if (!responseData.IsError && responseData.StatusCode == HttpStatusCode.Unauthorized)
            {
                ObtainToken();
                authorizationParameters = GetAuthorizationParameters();
                errorWhenEmptyToken = GetResponseDataWithErrorWhenEmptyToken(authorizationParameters);
                if (errorWhenEmptyToken != null)
                    return errorWhenEmptyToken;
                responseData = await base.SendAsync(content, requestSettings, authorizationParameters, token).ConfigureAwait(false);
            }
            return responseData;
        }

        private ResponseData GetResponseDataWithErrorWhenEmptyToken(AuthorizationParameters authorizationParameters)
        {
            string str = authorizationParameters.Headers.Get("Authorization");
            if (!string.IsNullOrEmpty(str) && str.IndexOf(" ", StringComparison.Ordinal) != -1)
                return null;
            return new ResponseData()
            {
                IsError = true,
                ErrorMessage = "Cannot obtain access token. Request was canceled."
            };
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

        protected internal string GetTokenEndpoint(HttpClient httpClient, string authority)
        {
            DiscoveryResponse result = httpClient.GetDiscoveryDocumentAsync(authority).Result;
            if (!result.IsError)
                return result.TokenEndpoint;
            Log.Error("Authorization: Dicovery request failed with error: " + result.Error, this);
            return null;
        }


    }
}