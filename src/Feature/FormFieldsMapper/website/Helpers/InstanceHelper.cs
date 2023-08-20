using Sitecore.Reflection;
using System;
using System.Collections.Concurrent;

namespace SitecoreMods.Feature.FormFieldsMapper.Helpers
{
    internal static class InstanceHelper
    {
        private static readonly ConcurrentDictionary<string, Type> ModelTypes = new ConcurrentDictionary<string, Type>();

        internal static object CreateInstance(string modelType, params object[] parameters)
        {
            if (string.IsNullOrEmpty(modelType))
                return null;
            if (!ModelTypes.TryGetValue(modelType, out var type))
            {
                var typeInfo = ReflectionUtil.GetTypeInfo(modelType);
                if (typeInfo == null)
                    return null;
                ModelTypes.TryAdd(modelType, typeInfo);
                type = typeInfo;
            }
            return ReflectionUtil.CreateObject(type, parameters);
        }
    }
}