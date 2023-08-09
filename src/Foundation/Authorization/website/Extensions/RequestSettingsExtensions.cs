using SitecoreMods.Foundation.Authorization.Enums;
using SitecoreMods.Foundation.Authorization.Models;

namespace SitecoreMods.Foundation.Authorization.Extensions
{
    public static class RequestSettingsExtensions
    {
        public static void SetContentTypeHeader(this RequestSettings settings, SerializationType serializationType)
        {
            if (serializationType != SerializationType.Json && serializationType == SerializationType.Xml)
                settings.ContentTypeHeader = "application/xml";
            else
                settings.ContentTypeHeader = "application/json";
        }
    }
}