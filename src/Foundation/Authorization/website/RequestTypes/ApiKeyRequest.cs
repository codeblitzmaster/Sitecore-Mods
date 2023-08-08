using System;
using System.Collections.Generic;
using System.Net.Http;
using Sitecore.Abstractions;
using SitecoreMods.Foundation.Authorization.Extensions;
using SitecoreMods.Foundation.Authorization.Models;

namespace SitecoreMods.Foundation.Authorization.RequestTypes
{
    public class ApiKeyRequest : BaseRequest
    {
        private const string KeyFieldName = "Key";
        private const string ValueFieldName = "Value";
        private const string AddToFieldName = "AddTo";
        private const string HeaderFieldName = "Header";
        private readonly IReadOnlyDictionary<string, string> _authProperties;

        public ApiKeyRequest(IReadOnlyDictionary<string, string> authProperties, IHttpClientFactory httpClientFactory, BaseLog log) : base(httpClientFactory, log)
        {
            _authProperties = authProperties;
        }

        protected override IEnumerable<string> ValidateAuthProperties()
        {
            List<string> errors = new List<string>();
            _authProperties.CheckPropertyIsNotNullOrEmpty(errors, KeyFieldName);
            _authProperties.CheckPropertyIsNotNullOrEmpty(errors, ValueFieldName);
            return errors;
        }

        protected override AuthorizationParameters GetAuthorizationParameters()
        {
            AuthorizationParameters authorizationParameters = new AuthorizationParameters();
            if ((this._authProperties.GetPropertyValue(AddToFieldName) ?? HeaderFieldName).Equals("Header", StringComparison.OrdinalIgnoreCase))
                authorizationParameters.Headers.Add(this._authProperties.GetPropertyValue(KeyFieldName), this._authProperties.GetPropertyValue(ValueFieldName));
            else
                authorizationParameters.QueryParameters.Add(this._authProperties.GetPropertyValue(KeyFieldName), this._authProperties.GetPropertyValue(ValueFieldName));
            return authorizationParameters;
        }
    }
}