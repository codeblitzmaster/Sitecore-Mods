using SitecoreMods.Foundation.Authorization.Enums;
using SitecoreMods.Foundation.Authorization.Models;

namespace SitecoreMods.Foundation.Authorization.Extensions
{
    public static class RequestSettingsExtensions
    {
        public static void SetContentTypeHeader(this RequestSettings settings)
        {
            if (string.IsNullOrWhiteSpace(settings.ContentType))
            {
                settings.ContentTypeHeader = "application/json";
            }
            else
            {
                settings.ContentTypeHeader = settings.ContentType;
            }
        }
    }
}