namespace Sitecore.Support.ExperienceEditor.Speak.Ribbon.Requests.SaveItem
{
    using Sitecore;
    using Sitecore.Data;
    using Sitecore.Data.Fields;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Sitecore.ExperienceEditor.Speak.Server.Responses;
    using Sitecore.Globalization;
    using Sitecore.Pipelines.Save;
    using Sitecore.Support.ExperienceEditor.Speak.Server.Requests;
    using Sitecore.ExperienceEditor.Speak.Server.Contexts;
    using System.Linq;
    using System.Text;

    public class CheckTemplateFieldChange : PipelineProcessorRequest<PageContext>
    {
        protected SaveArgs.SaveField GetField(SaveArgs.SaveItem saveItem, ID id)
        {
            Assert.ArgumentNotNull(saveItem, "saveItem");
            Assert.ArgumentNotNull(id, "id");
            return saveItem.Fields.FirstOrDefault<SaveArgs.SaveField>(saveField => (saveField.ID == id));
        }

        public override PipelineProcessorResponseValue ProcessRequest()
        {
            PipelineProcessorResponseValue value2 = new PipelineProcessorResponseValue();
            Item item = base.RequestContext.Item.Database.GetItem(base.RequestContext.ItemId, Language.Parse(base.RequestContext.Language), Sitecore.Data.Version.Parse(base.RequestContext.Version));
            if ((item != null) && (item.TemplateID == TemplateIDs.TemplateField))
            {
                SaveArgs.SaveItem saveItem = base.RequestContext.GetSaveArgs().Items[0];
                SaveArgs.SaveField field = this.GetField(saveItem, TemplateFieldIDs.Shared);
                SaveArgs.SaveField field2 = this.GetField(saveItem, TemplateFieldIDs.Unversioned);
                Field field3 = item.Fields[TemplateFieldIDs.Shared];
                Field field4 = item.Fields[TemplateFieldIDs.Unversioned];
                bool flag = false;
                if ((field != null) && (field3.Value != field.Value))
                {
                    flag = true;
                }
                if ((field2 != null) && (field4.Value != field2.Value))
                {
                    flag = true;
                }
                if (flag)
                {
                    StringBuilder builder = new StringBuilder(Translate.Text("You have changed the unversioned or shared flag.\n\n"));
                    builder.Append(Translate.Text("Enabling either of these flags will initiate a background process\nto update all the items based on this template.\n\nThis process might demand a lot of resources.\n\nIf you have enabled either the unversioned or shared flag, the previous version values of these\nfields will be lost.\n\nAre you sure you want to proceed?"));
                    value2.ConfirmMessage = builder.ToString();
                }
            }
            return value2;
        }
    }
}

