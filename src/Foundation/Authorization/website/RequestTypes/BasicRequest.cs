using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Sitecore.Abstractions;
using SitecoreMods.Foundation.Authorization.Extensions;
using SitecoreMods.Foundation.Authorization.Models;

namespace SitecoreMods.Foundation.Authorization.RequestTypes
{
    public class BasicRequest:BaseRequest
    {
        private const string UsernameFieldName = "Username";
        private const string PasswordFieldName = "Password";
        private readonly IReadOnlyDictionary<string, string> _authProperties;

        public BasicRequest(IReadOnlyDictionary<string, string> authProperties, IHttpClientFactory httpClientFactory,
            BaseLog log) : base(httpClientFactory, log)
        {
            _authProperties = authProperties;
        }

        protected override IEnumerable<string> ValidateAuthProperties()
        {
            List<string> errors = new List<string>();
            _authProperties.CheckPropertyIsNotNullOrEmpty((ICollection<string>)errors, UsernameFieldName);
            _authProperties.CheckPropertyIsNotNullOrEmpty((ICollection<string>)errors, PasswordFieldName);
            this._authProperties.CheckPropertyDoesNotHaveSemicolonSymbol((ICollection<string>)errors, UsernameFieldName);
            this._authProperties.CheckPropertyDoesNotHaveSemicolonSymbol((ICollection<string>)errors, PasswordFieldName);
            return (IEnumerable<string>)errors;
        }

        protected override AuthorizationParameters GetAuthorizationParameters()
        {
            AuthorizationParameters authorizationParameters = new AuthorizationParameters();
            Encoding ascii = Encoding.ASCII;
            string username = _authProperties?.GetPropertyValue(UsernameFieldName);
            string password = _authProperties?.GetPropertyValue(PasswordFieldName);
            string credentials = username + ":" + password;
            string base64Credentials = System.Convert.ToBase64String(ascii.GetBytes(credentials));
            authorizationParameters.Headers.Add("Authorization", "Basic " + base64Credentials);
            return authorizationParameters;
        }
    }
}