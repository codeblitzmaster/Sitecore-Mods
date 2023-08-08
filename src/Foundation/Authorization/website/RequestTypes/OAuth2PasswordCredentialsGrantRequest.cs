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
    public class OAuth2PasswordCredentialsGrantRequest: BaseOAuth2Request
    {
        private const string AuthorityUrlFieldName = "AuthorityURL";
        private const string ClientIdFieldName = "ClientID";
        private const string ClientSecretFieldName = "ClientSecret";
        private const string ScopeFieldName = "Scope";
        private const string HeaderPrefixFieldName = "HeaderPrefix";
        private const string UsernameFieldName = "Username";
        private const string PasswordFieldName = "Password";
        private readonly object _tokenLocker = new object();
        private readonly IReadOnlyDictionary<string, string> _authProperties;
        private readonly Lazy<HttpClient> _tokenClient;
        private string _accessToken;

        public OAuth2PasswordCredentialsGrantRequest(IReadOnlyDictionary<string, string> authProperties,IHttpClientFactory httpClientFactory, BaseLog log) : base(httpClientFactory, log)
        {
            _authProperties = authProperties;
            _tokenClient = new Lazy<HttpClient>();
        }

        protected override IEnumerable<string> ValidateAuthProperties()
        {
            List<string> errors = new List<string>();
            _authProperties.CheckPropertyIsUrl(errors, AuthorityUrlFieldName);
            _authProperties.CheckPropertyIsNotNullOrEmpty(errors, ClientIdFieldName);
            _authProperties.CheckPropertyIsNotNullOrEmpty(errors, ClientSecretFieldName);
            _authProperties.CheckPropertyIsNotNullOrEmpty(errors, HeaderPrefixFieldName);
            _authProperties.CheckPropertyIsNotNullOrEmpty(errors, UsernameFieldName);
            _authProperties.CheckPropertyIsNotNullOrEmpty(errors, PasswordFieldName);
            return errors;
        }

        protected override AuthorizationParameters GetAuthorizationParameters() => GetAuthorizationParameters(_accessToken, _authProperties.GetPropertyValue(HeaderPrefixFieldName));

        protected override string ObtainToken()
        {
            lock (_tokenLocker)
            {
                string tokenEndpoint = GetTokenEndpoint(this._tokenClient.Value, _authProperties.GetPropertyValue(AuthorityUrlFieldName));
                if (tokenEndpoint == null)
                {
                    _accessToken = null;
                    return _accessToken;
                }

                HttpClient client = _tokenClient.Value;

                PasswordTokenRequest request = new PasswordTokenRequest()
                {
                    Address = tokenEndpoint,
                    ClientId = _authProperties.GetPropertyValue(ClientIdFieldName),
                    ClientSecret = _authProperties.GetPropertyValue(ClientSecretFieldName),
                    Scope = _authProperties.GetPropertyValue(ScopeFieldName),
                    UserName = _authProperties.GetPropertyValue(UsernameFieldName),
                    Password = _authProperties.GetPropertyValue(PasswordFieldName)
                };
                CancellationToken cancellationToken = new CancellationToken();
                TokenResponse result = client.RequestPasswordTokenAsync(request, cancellationToken).Result;
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