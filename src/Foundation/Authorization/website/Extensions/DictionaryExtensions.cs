using Sitecore.Web;
using System.Collections.Generic;

namespace SitecoreMods.Foundation.Authorization.Extensions
{
    /// <summary>Dictionary extensions.</summary>
    public static class DictionaryExtensions
    {
        /// <summary>Get the value of property.</summary>
        /// <param name="properties">The properties.</param>
        /// <param name="propertyName">The property name.</param>
        /// <returns></returns>
        public static string GetPropertyValue(
          this IReadOnlyDictionary<string, string> properties,
          string propertyName)
        {
            if (properties == null)
                return (string)null;
            string str;
            return !properties.TryGetValue(propertyName, out str) ? (string)null : str;
        }

        public static void CheckPropertyDoesNotHaveSemicolonSymbol(
          this IReadOnlyDictionary<string, string> properties,
          ICollection<string> errors,
          string propertyName)
        {
            string propertyValue = properties != null ? properties.GetPropertyValue(propertyName) : (string)null;
            if (propertyValue == null || !propertyValue.Contains(":"))
                return;
            errors.Add("Property '" + propertyName + "' contains illegal character ':'.");
        }

        public static void CheckPropertyIsNotNullOrEmpty(
          this IReadOnlyDictionary<string, string> properties,
          ICollection<string> errors,
          string propertyName)
        {
            if (properties == null)
                errors.Add("Properties dictionary is null.");
            else if (!properties.ContainsKey(propertyName))
            {
                errors.Add("Properties dictionary does not contain property '" + propertyName + "'.");
            }
            else
            {
                if (!string.IsNullOrEmpty(properties.GetPropertyValue(propertyName)))
                    return;
                errors.Add("Property '" + propertyName + "' is null or empty.");
            }
        }

        public static void CheckPropertyIsUrl(
          this IReadOnlyDictionary<string, string> properties,
          ICollection<string> errors,
          string propertyName)
        {
            if (WebUtil.IsExternalUrl(properties != null ? properties.GetPropertyValue(propertyName) : (string)null))
                return;
            errors.Add("Property '" + propertyName + "' does not contain valid Url.");
        }
    }
}