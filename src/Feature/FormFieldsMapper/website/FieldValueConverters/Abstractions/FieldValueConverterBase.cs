using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Sitecore.DependencyInjection;
using Sitecore.Diagnostics;
using Sitecore.ExperienceForms.Diagnostics;
using Sitecore.ExperienceForms.Models;
using System;
using System.Text.RegularExpressions;

namespace SitecoreMods.Feature.FormFieldsMapper.FieldValueConverters.Abstractions
{
    public abstract class FieldValueConverterBase<TParameters>: IFieldValueConverter
    {
        private ILogger _logger;

        protected virtual ILogger Logger => this._logger ?? (this._logger = ServiceLocator.ServiceProvider.GetService<ILogger>());

        protected TParameters Parameters { get; set; }

        protected FieldValueConverterBase(string parameters)
        {
            TParameters target;
            if(TryParse(parameters, out target))
            {
                Parameters = target;
            }
        }

        public abstract object ConvertFieldValue(object fieldValue);

        protected virtual bool TryParse(string value, out TParameters target)
        {
            if (string.IsNullOrEmpty(value))
            {
                target = default(TParameters);
                return false;
            }
            try
            {
                target = JsonConvert.DeserializeObject<TParameters>(value);
            }
            catch (JsonException ex)
            {
                this.Logger.LogError(ex.Message, (Exception)ex, (object)this);
                target = default(TParameters);
                return false;
            }
            return (object)target != null;
        }
    }
}