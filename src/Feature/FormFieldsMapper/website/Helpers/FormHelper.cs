using Sitecore.Diagnostics;
using Sitecore.ExperienceForms.Models;
using Sitecore.ExperienceForms.Processing;
using Sitecore.Reflection;
using SitecoreMods.Feature.FormFieldsMapper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace SitecoreMods.Feature.FormFieldsMapper.Helpers
{
    public static partial class FormHelper
    {
        private const string AtMentionRegexFormat = "@\\[(.*?)\\]";
        
        public static string GetAtMentionedParsedValue(this Field field, IList<IViewModel> formSubmitContextFields)
        {
            var value = field.Value;
            var matches = Regex.Matches(value, AtMentionRegexFormat, RegexOptions.IgnoreCase);
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    if (match.Success)
                    {
                        var fieldName = match.Groups[1].Value;
                        var formSubmitContextField = formSubmitContextFields.FirstOrDefault(x => x.Name == fieldName);
                        if(formSubmitContextField!= null)
                        {
                            value = value.Replace(string.Format("{0}{1}{2}", "@[", fieldName, "]"), GetFieldValue(formSubmitContextField));
                        }
                    }
                }
            }
            return value;
        }

        public static string GetFieldValue(IViewModel postedField)
        {
            Assert.ArgumentNotNull((object)postedField, nameof(postedField));
            return ReflectionUtil.CallMethod((object)postedField, "GetStringValue").ToString();
        }
    }
}