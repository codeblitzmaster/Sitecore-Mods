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
    /// <summary>Data received in http response.</summary>
    public class ResponseData
    {
        /// <summary>
        /// Gets a value indicating whether the http request finished with a success status code.
        /// </summary>
        public bool IsSuccessStatusCode => this.StatusCode >= HttpStatusCode.OK && this.StatusCode <= (HttpStatusCode)299;

        /// <summary>Gets or sets the content.</summary>
        /// <value>The content.</value>
        public HttpContent Content { get; set; }

        /// <summary>Gets or sets the status code.</summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>Gets or sets the headers.</summary>
        public HttpResponseHeaders Headers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether request finished with an error.
        /// </summary>
        public bool IsError { get; set; }

        /// <summary>Gets or sets the error message.</summary>
        public string ErrorMessage { get; set; }

        /// <summary>Gets or sets the exception.</summary>
        public Exception Exception { get; set; }
    }
}
