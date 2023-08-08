using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Sitecore.Abstractions;
using Sitecore.Common.HttpClient;
using SitecoreMods.Foundation.Authorization.Models;
using ResponseData = SitecoreMods.Foundation.Authorization.Models.ResponseData;

namespace SitecoreMods.Foundation.Authorization.RequestTypes
{
    public abstract class BaseRequest: IDisposable
    {
        private bool _disposed;

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly BaseLog _log;
        protected BaseLog Log { get; }


        protected BaseRequest(IHttpClientFactory httpClientFactory, BaseLog log)
        {
            _httpClientFactory = httpClientFactory;
            _log = log;
        }

        public async Task<ResponseData> SendRequestAsync(RequestSettings requestSettings, string content, CancellationToken token)
        {
            var httpClient = _httpClientFactory.CreateClient("ApiClient");
            var request = new HttpRequestMessage(new HttpMethod(requestSettings.Method), requestSettings.Url);
            return new ResponseData()
            {
                IsError = true,
                ErrorMessage = "Not Implemented"
            };
        }

        protected abstract IEnumerable<string> ValidateAuthProperties();

        protected abstract AuthorizationParameters GetAuthorizationParameters();

        public void Dispose()
        {
            //Dispose(true);
            GC.SuppressFinalize(this);
        }

        //protected virtual void Dispose(bool disposing)
        //{
        //    if (this._disposed)
        //        return;
        //    if (disposing && this._httpClient.IsValueCreated)
        //        this._httpClient.Value.Dispose();
        //    this._disposed = true;
        //}
    }
}