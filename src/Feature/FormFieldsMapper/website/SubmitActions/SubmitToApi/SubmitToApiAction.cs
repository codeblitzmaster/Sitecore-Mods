using Sitecore.Diagnostics;
using Sitecore.ExperienceForms.Diagnostics;
using Sitecore.ExperienceForms.Models;
using Sitecore.ExperienceForms.Processing;
using Sitecore.ExperienceForms.Processing.Actions;
using SitecoreMods.Feature.FormFieldsMapper.Helpers;
using SitecoreMods.Feature.FormFieldsMapper.Models;
using System.Collections.Generic;
using System.Linq;
using SitecoreMods.Foundation.Authorization.Interfaces;
using static System.FormattableString;
using Sitecore.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;


namespace SitecoreMods.Feature.FormFieldsMapper.SubmitActions.SubmitToApi
{
    public class SubmitToApiAction : SubmitActionBase<SubmitToApiActionData>
    {
        private readonly ILogger _logger;
        private readonly IApiIntegrationService _apiIntegrationService;
        /// <summary>
        /// Initializes a new instance of the <see cref="SubmitToApiAction"/> class.
        /// </summary>
        /// <param name="submitActionData">The submit action data.</param>
        public SubmitToApiAction(ISubmitActionData submitActionData) : this(submitActionData, ServiceLocator.ServiceProvider.GetService<IApiIntegrationService>(), ServiceLocator.ServiceProvider.GetService<ILogger>())
        {
        }

        public SubmitToApiAction(ISubmitActionData submitActionData, IApiIntegrationService apiIntegrationService, ILogger logger) : base(submitActionData)
        {
            _apiIntegrationService = apiIntegrationService;
            _logger = logger;
        }

        protected override bool Execute(SubmitToApiActionData data, FormSubmitContext formSubmitContext)
        {
            Assert.ArgumentNotNull(formSubmitContext, nameof(formSubmitContext));
            if (!formSubmitContext.HasErrors)
            {
                Logger.Info(Invariant($"Form {formSubmitContext.FormId} submitted successfully."), this);
            }

            var postData = new Dictionary<string, object>();

            var nonEmptyFields = data.Mappings.Where(x => !string.IsNullOrWhiteSpace(x.Value));

            foreach (Field field in nonEmptyFields)
            {
                var value = field.GetAtMentionedParsedValue(formSubmitContext);
                postData.Add(field.Name, value);
            }

            var taskResponse  = _apiIntegrationService.FireAsync(data.ApiEndpointId, postData);
            taskResponse.ContinueWith((task) =>
            {
                if (task.IsFaulted)
                {
                    _logger.LogError(task.Exception?.ToString(), "Error while submitting form data to API");
                }

                if (task.IsCompleted)
                {
                    var result = task.Result;
                    if (result != null)
                    {
                        _logger.Info($"Form ({formSubmitContext.FormId}) data submitted to API successfully \n Response: \n {result.Content}");
                        // Do something else with result if required
                    }
                }
            });

            return true;
        }
    }
}