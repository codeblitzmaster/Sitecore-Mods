using Sitecore.Mvc.Presentation;
using Sitecore.Speak.Components.Models;
using Sitecore.Web.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SitecoreMods.Feature.FormFieldsMapper.Models
{
    public class FormFieldsMapperRenderingModel : CollectionBaseRenderingModel
    {
        public override void Initialize(Rendering rendering)
        {
            base.Initialize(rendering);
            Requires.Clear();
            Requires.Add(new RequireDescriptor(RequireType.Css, "client", "Applications/FormsBuilder/Layouts/Renderings/SitecoreMods/FormFieldsMapper/css/vendor/mentionsInput.css"));
            Requires.Add(new RequireDescriptor(RequireType.Css, "client", "Applications/FormsBuilder/Layouts/Renderings/SitecoreMods/FormFieldsMapper/css/FormFieldsMapper.css"));
            Requires.Add(new RequireDescriptor(RequireType.JavaScript, "client", "Applications/FormsBuilder/Layouts/Renderings/SitecoreMods/FormFieldsMapper/js/FormFieldsMapper.js"));
        }
    }
}