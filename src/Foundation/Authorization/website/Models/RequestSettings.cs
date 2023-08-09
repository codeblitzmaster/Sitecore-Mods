using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SitecoreMods.Foundation.Authorization.Models
{
    public class RequestSettings
    {
        public string Url { get; set; }
        public string Method { get; set; } = "POST";
        public string ContentTypeHeader { get; set; }
        public string ContentType { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public Dictionary<string, string> QueryString { get; set; }
        public Dictionary<string, string> Body { get; set; }
        public RequestSettings() { }
        public RequestSettings(string url, string method, string contentType, Dictionary<string, string> headers, Dictionary<string, string> queryString, Dictionary<string, string> body)
        {
            Url = url;
            Method = method;
            ContentType = contentType;
            Headers = headers;
            QueryString = queryString;
            Body = body;
        }


    }
}
