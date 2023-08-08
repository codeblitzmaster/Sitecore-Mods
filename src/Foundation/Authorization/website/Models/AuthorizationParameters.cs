using System.Collections.Specialized;
using System.Net;
using System.Web;

namespace SitecoreMods.Foundation.Authorization.Models
{
    public class AuthorizationParameters
    {
        public static AuthorizationParameters EmptyParameters { get; } = new AuthorizationParameters();

        public WebHeaderCollection Headers { get; } = new WebHeaderCollection();

        public NameValueCollection QueryParameters { get; } = HttpUtility.ParseQueryString(string.Empty);
    }
}