using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Sitecore.Web;
using System.Collections.Generic;
using Newtonsoft.Json.Converters;

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

        public static string ExpandFlattenedObjectToJsonString(this Dictionary<string, object> flattenedObject)
        {
            //logic to expand the flattened object of sample mappings to nested object recursively
            var nestedObject = new Dictionary<string, object>();

            foreach (var entry in flattenedObject)
            {
                var keys = entry.Key.Split('.');
                var currentValue = entry.Value;

                Dictionary<string, object> currentLevel = nestedObject;

                for (int i = 0; i < keys.Length - 1; i++)
                {
                    string key = keys[i];
                    if (key.EndsWith("]"))
                    {
                        int startIndex = key.LastIndexOf('[');
                        string arrayKey = key.Substring(0, startIndex);
                        int arrayIndex = int.Parse(key.Substring(startIndex + 1, key.Length - startIndex - 2));

                        if (!currentLevel.ContainsKey(arrayKey))
                        {
                            currentLevel[arrayKey] = new List<object>();
                        }
                        var array = (List<object>)currentLevel[arrayKey];

                        while (array.Count <= arrayIndex)
                        {
                            array.Add(new Dictionary<string, object>());
                        }

                        if (!(array[arrayIndex] is Dictionary<string, object>))
                        {
                            array[arrayIndex] = new Dictionary<string, object>();
                        }

                        currentLevel = (Dictionary<string, object>)array[arrayIndex];
                    }
                    else
                    {
                        if (!currentLevel.ContainsKey(key))
                        {
                            currentLevel[key] = new Dictionary<string, object>();
                        }
                        currentLevel = (Dictionary<string, object>)currentLevel[key];
                    }
                }

                if (currentValue is string valueString)
                {
                    if (IsJson(valueString))
                    {
                        currentValue = TryParseJson(valueString);
                    }
                }

                string lastKey = keys[keys.Length - 1];
                if (lastKey.EndsWith("]"))
                {
                    int startIndex = lastKey.LastIndexOf('[');
                    string arrayKey = lastKey.Substring(0, startIndex);
                    int arrayIndex = int.Parse(lastKey.Substring(startIndex + 1, lastKey.Length - startIndex - 2));

                    if (!currentLevel.ContainsKey(arrayKey))
                    {
                        currentLevel[arrayKey] = new List<object>();
                    }
                    var array = (List<object>)currentLevel[arrayKey];

                    while (array.Count <= arrayIndex)
                    {
                        array.Add(new Dictionary<string, object>());
                    }

                    if (!(array[arrayIndex] is Dictionary<string, object>))
                    {
                        array[arrayIndex] = new Dictionary<string, object>();
                    }

                    array[arrayIndex] = currentValue;
                }
                else
                {
                    currentLevel[lastKey] = currentValue;
                }
            }
            var jsonString = JsonConvert.SerializeObject(nestedObject, Formatting.Indented, new ExpandoObjectConverter());
            return jsonString;
        }

        private static bool IsJson(string value)
        {
            try
            {
                JToken.Parse(value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static JToken TryParseJson(string value)
        {
            try
            {
                return JToken.Parse(value);
            }
            catch (JsonReaderException)
            {
                return value; // Parsing failed, not a valid JSON
            }
        }

    }
}