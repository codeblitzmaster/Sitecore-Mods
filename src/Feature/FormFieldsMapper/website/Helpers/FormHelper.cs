using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.ExperienceForms.Models;
using Sitecore.ExperienceForms.Processing;
using Sitecore.Reflection;
using SitecoreMods.Feature.FormFieldsMapper.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace SitecoreMods.Feature.FormFieldsMapper.Helpers
{
    public static partial class FormHelper
    {
        private const string AtMentionRegexFormat = "@\\[(.*?)\\]";

        public static string GetAtMentionedParsedValue(this Field field, FormSubmitContext formSubmitContext)
        {
            IList<IViewModel> formSubmitContextFields = formSubmitContext.Fields;
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
                        if (formSubmitContextField != null)
                        {
                            value = value.Replace(string.Format("{0}{1}{2}", "@[", fieldName, "]"), GetFieldValue(formSubmitContextField));
                        }
                    }
                }
            }
            value = GetParsedTokenValue(value, formSubmitContext);
            return value;
        }

        public static string GetFieldValue(IViewModel postedField)
        {
            Assert.ArgumentNotNull((object)postedField, nameof(postedField));
            return ReflectionUtil.CallMethod((object)postedField, "GetStringValue").ToString();
        }

        public static string GetParsedTokenValue(string value, FormSubmitContext formSubmitContext)
        {
            var updatedValue = value;
            if (value.Contains("$timenow"))
            {
                updatedValue = updatedValue.Replace("$timenow", DateTime.Now.ToString(CultureInfo.InvariantCulture));
            }

            if (value.Contains("$timeutcnow"))
            {
                updatedValue = updatedValue.Replace("$timeutcnow", DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
            }

            if (value.Contains("$guid"))
            {
                updatedValue = updatedValue.Replace("$guid", Guid.NewGuid().ToString());
            }

            if (value.Contains("$formid"))
            {
                updatedValue = updatedValue.Replace("$formid", formSubmitContext.FormId.ToString());
            }

            if (value.Contains("$formname"))
            {
                var formItem = Sitecore.Context.Database.GetItem(new ID(formSubmitContext.FormId));
                if (formItem != null)
                {
                    var formName = string.IsNullOrWhiteSpace(formItem.DisplayName) ? formItem.Name : formItem.DisplayName;
                    updatedValue = updatedValue.Replace("$formname", formName);
                }
            }
            return updatedValue;
        }
    }
}