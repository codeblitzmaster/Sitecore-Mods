using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Mvc.Presentation;
using Sitecore.Speak.Components.Models;
using Sitecore.Web.UI.Controls.Common.CompositeControls;
using System.Globalization;
using System.Web;

namespace SitecoreMods.Feature.FormFieldsMapper.Models
{
    public class FormFieldsMapperRenderingModel : CollectionBaseRenderingModel
    {
        private CompositeControl composite;
        private Item layoutItem;

        public HtmlString Layout
        {
            get
            {
                using (new ContextItemSwitcher(this.layoutItem))
                    return new HtmlString(this.composite.Render());
            }
        }

        public override void Initialize(Rendering rendering)
        {
            if (rendering == null)
                return;
            base.Initialize(rendering);
            this.HasNestedControls = true;
            this.layoutItem = Context.Database.GetItem(rendering.RenderingItem.ID);
            this.AppendCssClass();
            this.AssignJavascript();
            CompositeControl compositeControl = new CompositeControl(rendering, rendering.RenderingItem.ID.ToString(), (string)null);
            compositeControl.UsePlaceholders = true;
            this.composite = compositeControl;
        }

        protected virtual void AppendCssClass() => this.Class.Append("sc-" + this.layoutItem.Name.ToLower(CultureInfo.InvariantCulture));

        protected virtual void AssignJavascript() => this.Requires.Script("client", this.layoutItem.Name + ".js");
    }
}