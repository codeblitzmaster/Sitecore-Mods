using System.Collections.Generic;
using System;
using System.Net.Http;
using Sitecore.Abstractions;
using SitecoreMods.Foundation.Authorization.Extensions;
using SitecoreMods.Foundation.Authorization.Models;
using IdentityModel.Client;
using System.Threading;

namespace SitecoreMods.Foundation.Authorization.RequestTypes
{
    public class OAuth2ClientCredentialsGrantRequest : BaseOAuth2Request
    {
        private const string AuthorityUrlFieldName = "AuthorityURL";
        private const string ClientIdFieldName = "ClientID";
        private const string ClientSecretFieldName = "ClientSecret";
        private const string ScopeFieldName = "Scope";
        private const string HeaderPrefixFieldName = "HeaderPrefix";
        private readonly IReadOnlyDictionary<string, string> _authProperties;
        //private readonly Lazy<HttpClient> _tokenClient;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly object _tokenLocker = new object();
        private string _accessToken;

        public OAuth2ClientCredentialsGrantRequest(IReadOnlyDictionary<string, string> authProperties, IHttpClientFactory httpClientFactory, BaseLog log):base(httpClientFactory, log)
        {
            _httpClientFactory = httpClientFactory;
            _authProperties = authProperties;
            //this._tokenClient = new Lazy<HttpClient>();
        }

        protected override IEnumerable<string> ValidateAuthProperties()
        {
            List<string> errors = new List<string>();
            _authProperties.CheckPropertyIsUrl(errors, AuthorityUrlFieldName);
            _authProperties.CheckPropertyIsNotNullOrEmpty(errors, ClientIdFieldName);
            _authProperties.CheckPropertyIsNotNullOrEmpty(errors, ClientSecretFieldName);
            _authProperties.CheckPropertyIsNotNullOrEmpty(errors, HeaderPrefixFieldName);
            return errors;
        }

        protected override AuthorizationParameters GetAuthorizationParameters() => GetAuthorizationParameters(_accessToken, _authProperties.GetPropertyValue(HeaderPrefixFieldName));

        protected override string ObtainToken()
        {
            lock (_tokenLocker)
            {
                var httpClient = _httpClientFactory.CreateClient();
                string tokenEndpoint = GetTokenEndpoint(httpClient, _authProperties.GetPropertyValue(AuthorityUrlFieldName));
                if (tokenEndpoint == null)
                {
                    _accessToken = null;
                    return _accessToken;
                }

                //HttpClient client = _tokenClient.Value;

                ClientCredentialsTokenRequest request = new ClientCredentialsTokenRequest
                {
                    Address = tokenEndpoint,
                    ClientId = _authProperties.GetPropertyValue(ClientIdFieldName),
                    ClientSecret = _authProperties.GetPropertyValue(ClientSecretFieldName),
                    Scope = _authProperties.GetPropertyValue(ScopeFieldName)
                };
                CancellationToken cancellationToken = new CancellationToken();
                TokenResponse result = httpClient.RequestClientCredentialsTokenAsync(request, cancellationToken).Result;
                if (result.IsError)
                {
                    Log.Error("Authorization: Token request failed with error: " + result.Error, this);
                    _accessToken = null;
                    return _accessToken;
                }
                _accessToken = result.AccessToken;
                return _accessToken;
            }
        }
    }
}