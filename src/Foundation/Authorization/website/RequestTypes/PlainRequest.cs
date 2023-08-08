using System.Collections.Generic;
using System;
using System.Net.Http;
using Sitecore.Abstractions;
using SitecoreMods.Foundation.Authorization.Models;

namespace SitecoreMods.Foundation.Authorization.RequestTypes
{
    public class PlainRequest: BaseRequest
    {
        public PlainRequest(IHttpClientFactory httpClientFactory, BaseLog log) : base(httpClientFactory, log)
        {
        }

        protected override IEnumerable<string> ValidateAuthProperties() => (IEnumerable<string>)Array.Empty<string>();

        protected override AuthorizationParameters GetAuthorizationParameters() => AuthorizationParameters.EmptyParameters;
    }
}