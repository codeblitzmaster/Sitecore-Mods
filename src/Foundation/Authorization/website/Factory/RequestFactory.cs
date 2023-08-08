using System;
using System.Collections.Generic;
using System.Net.Http;
using Sitecore.Abstractions;
using Sitecore.DynamicSerialization;
using SitecoreMods.Foundation.Authorization.Enums;
using SitecoreMods.Foundation.Authorization.Models;
using SitecoreMods.Foundation.Authorization.RequestTypes;

namespace SitecoreMods.Foundation.Authorization.Factory
{
    public class RequestFactory
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly BaseLog _log;

        public RequestFactory(IHttpClientFactory httpClientFactory,BaseLog log)
        {
            _httpClientFactory = httpClientFactory;
            _log = log;
        }

        public virtual BaseRequest GetRequestHandler(AuthorizationSettings authorizationSettings)
        {
            BaseRequest baseRequest;
            switch (authorizationSettings.AuthorizationType)
            {
                case AuthorizationTypes.None:
                    baseRequest = new PlainRequest(_httpClientFactory, _log);
                    break;

                case AuthorizationTypes.ApiKey:
                    baseRequest = new ApiKeyRequest(authorizationSettings.AuthProperties, _httpClientFactory, _log);
                    break;

                case AuthorizationTypes.Basic:
                    baseRequest = new BasicRequest(authorizationSettings.AuthProperties, _httpClientFactory, _log);
                    break;

                case AuthorizationTypes.OAuth2ClientCredentialsGrant:
                    baseRequest = new OAuth2ClientCredentialsGrantRequest(authorizationSettings.AuthProperties, _httpClientFactory, _log);
                    break;

                case AuthorizationTypes.OAuth2PasswordCredentialsGrant:
                    baseRequest = new OAuth2PasswordCredentialsGrantRequest(authorizationSettings.AuthProperties, _httpClientFactory, _log);
                    break;

                default:
                    throw new ArgumentOutOfRangeException($"Unknown Authorization Type: {authorizationSettings.AuthorizationType}");
            }

            return baseRequest;
        }
    }
}