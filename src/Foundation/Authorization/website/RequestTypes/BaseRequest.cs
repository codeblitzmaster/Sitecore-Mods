using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Sitecore.Abstractions;
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
        public HttpClient HttpClient;


        protected BaseRequest(IHttpClientFactory httpClientFactory, BaseLog log)
        {
            _httpClientFactory = httpClientFactory;
            _log = log;
        }

        public async Task<ResponseData> SendRequestAsync(RequestSettings requestSettings, string content, CancellationToken token)
        {
            HttpClient = _httpClientFactory.CreateClient();

            var errors = ValidateAuthProperties()?.ToList() ?? throw new ArgumentNullException("ValidateAuthProperties()");
            if (errors.Any())
            {
                string message = "::SitecoreMods:Authorization:: Cannot validate authorization properties. Errors: '" + string.Join(";",errors) + "'";
                _log.Error(message, this);
                return new ResponseData()
                {
                    IsError = true,
                    ErrorMessage = message
                };
            }
            return await SendAsync(content, requestSettings, GetAuthorizationParameters(), token).ConfigureAwait(false);
        }

        public virtual async Task<ResponseData> SendAsync(string content, RequestSettings requestSettings, AuthorizationParameters authorizationParameters, CancellationToken cancellationToken)
        {
            var requestUrlString = requestSettings.Url + (authorizationParameters.QueryParameters.Count > 0 ? "?" + authorizationParameters.QueryParameters : string.Empty);
            var requestUrl = new Uri(requestUrlString);
            HttpMethod httpMethod = new HttpMethod(requestSettings.Method);
            
            HttpRequestMessage request = new HttpRequestMessage(httpMethod, requestUrl);

            foreach (var key in authorizationParameters.Headers.AllKeys)
            {
                request.Headers.Add(key, authorizationParameters.Headers[key]);
            }

            if (httpMethod != HttpMethod.Get)
            {
                StringContent stringContent = new StringContent(content);
                if (!string.IsNullOrWhiteSpace(requestSettings.ContentTypeHeader))
                    stringContent.Headers.ContentType = new MediaTypeHeaderValue(requestSettings.ContentTypeHeader);
                request.Content = stringContent;
            }

            if (cancellationToken.IsCancellationRequested)
                return null;

            ResponseData response;
            try
            {
                var httpResponseMessage = HttpClient.SendAsync(request, cancellationToken).Result;//.ConfigureAwait(false);
                
                response = new ResponseData()
                {
                    StatusCode = httpResponseMessage.StatusCode,
                    IsError = !httpResponseMessage.IsSuccessStatusCode,
                    ErrorMessage = httpResponseMessage.ReasonPhrase,
                    HttpContent = httpResponseMessage.Content
                };

                var responseContent = await response.HttpContent.ReadAsStringAsync().ConfigureAwait(false);
                response.Content = responseContent;
                if (!response.IsSuccessStatusCode)
                {
                    _log.Error($"::SitecoreMods:Authorization:: Request not successful {requestUrlString}. Status Code: {response.StatusCode}", this);
                    _log.Error($"::SitecoreMods:Authorization:: Request not successful {requestUrlString}. Return Response Content: \n{responseContent}", this);
                }
                else
                {
                    _log.Info($"::SitecoreMods:Authorization:: Request Successful {requestUrlString}. Status Code: {response.StatusCode} \n Return Response Content: \n{responseContent}", this);
                }
            }
            catch (HttpRequestException ex)
            {
                _log.Error($"::SitecoreMods:Authorization:: Request failed {requestUrlString}", this);
                response = new ResponseData()
                {
                    IsError = true,
                    ErrorMessage = ex.Message,
                    Exception = (Exception)ex
                };
            }
            catch (TaskCanceledException ex)
            {
                string message = $"::SitecoreMods:Authorization:: Request timed out or cancelled by user. {requestUrlString}";
                _log.Error(message, this);
                response = new ResponseData()
                {
                    IsError = true,
                    ErrorMessage = message,
                    Exception = (Exception)ex
                };
            }
            return response;
        }

        protected abstract IEnumerable<string> ValidateAuthProperties();

        protected abstract AuthorizationParameters GetAuthorizationParameters();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
                HttpClient.Dispose();
            _disposed = true;
        }
    }
}