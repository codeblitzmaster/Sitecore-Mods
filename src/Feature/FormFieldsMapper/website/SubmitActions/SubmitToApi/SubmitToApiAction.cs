using Sitecore.Diagnostics;
using Sitecore.ExperienceForms.Diagnostics;
using Sitecore.ExperienceForms.Models;
using Sitecore.ExperienceForms.Processing;
using Sitecore.ExperienceForms.Processing.Actions;
using SitecoreMods.Feature.FormFieldsMapper.Helpers;
using SitecoreMods.Feature.FormFieldsMapper.Models;
using System.Collections.Generic;
using System.Linq;
using static System.FormattableString;

namespace SitecoreMods.Feature.FormFieldsMapper.SubmitActions.SubmitToApi
{
    public class SubmitToApiAction : SubmitActionBase<SubmitToApiActionData>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubmitToApiAction"/> class.
        /// </summary>
        /// <param name="submitActionData">The submit action data.</param>
        public SubmitToApiAction(ISubmitActionData submitActionData) : base(submitActionData)
        {
        }

        protected override bool Execute(SubmitToApiActionData data, FormSubmitContext formSubmitContext)
        {
            Assert.ArgumentNotNull(formSubmitContext, nameof(formSubmitContext));
            if (!formSubmitContext.HasErrors)
            {
                Logger.Info(Invariant($"Form {formSubmitContext.FormId} submitted successfully."), this);
            }

            var test = data;
            var postData = new Dictionary<string, object>();

            var nonEmptyFields = data.Mappings.Where(x => !string.IsNullOrWhiteSpace(x.Value));

            foreach (Field field in nonEmptyFields)
            {
                var value = field.GetAtMentionedParsedValue(formSubmitContext.Fields);
                postData.Add(field.Name, value);
            }

            return true;
        }
    }
}