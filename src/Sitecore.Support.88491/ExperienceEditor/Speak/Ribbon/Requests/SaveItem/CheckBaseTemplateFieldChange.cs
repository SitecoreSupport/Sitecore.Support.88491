namespace Sitecore.Support.ExperienceEditor.Speak.Ribbon.Requests.SaveItem
{
    using Sitecore;
    using Sitecore.Data.Fields;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Sitecore.ExperienceEditor.Speak.Server.Responses;
    using Sitecore.Globalization;
    using Sitecore.Support.ExperienceEditor.Speak.Server.Requests;
    using Sitecore.ExperienceEditor.Speak.Server.Contexts;
    using System;
    using System.Linq;

    public class CheckBaseTemplateFieldChange : PipelineProcessorRequest<PageContext>
    {
        internal bool AreBaseTemplatesRemoved(string baseTemplateFieldValue, string newFieldValue)
        {
            Assert.ArgumentNotNull(baseTemplateFieldValue, "baseTemplateFieldValue");
            Assert.ArgumentNotNull(newFieldValue, "newFieldValue");
            string[] first = newFieldValue.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            string[] second = baseTemplateFieldValue.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            return (first.Intersect<string>(second, StringComparer.InvariantCultureIgnoreCase).Count<string>() < second.Length);
        }

        public override PipelineProcessorResponseValue ProcessRequest()
        {
            PipelineProcessorResponseValue value2 = new PipelineProcessorResponseValue();
            Item item = base.RequestContext.Item.Database.GetItem(base.RequestContext.ItemId, Language.Parse(base.RequestContext.Language), Sitecore.Data.Version.Parse(base.RequestContext.Version));
            if ((item != null) && item.Database.Engines.TemplateEngine.IsTemplate(item))
            {
                Field field = item.Fields.FirstOrDefault<Field>(x => x.ID == FieldIDs.BaseTemplate);
                if ((field != null) && this.AreBaseTemplatesRemoved(item[FieldIDs.BaseTemplate], field.Value))
                {
                    value2.ConfirmMessage = Translate.Text("You are about to remove one or more base templates from the current template.\n\nWhen you remove a base template, Sitecore updates all the items based on the current template and clears any field values in these items that are associated with the fields in the base template that you removed. These field values cannot be restored once you have removed them.\n\nDo you want to proceed?");
                }
            }
            return value2;
        }
    }
}

