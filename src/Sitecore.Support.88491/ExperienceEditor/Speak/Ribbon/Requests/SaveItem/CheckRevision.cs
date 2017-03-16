namespace Sitecore.Support.ExperienceEditor.Speak.Ribbon.Requests.SaveItem
{
    using Sitecore;
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using Sitecore.ExperienceEditor.Speak.Server.Responses;
    using Sitecore.Globalization;
    using Sitecore.Pipelines.Save;
    using Sitecore.Support.ExperienceEditor.Speak.Server.Requests;
    using Sitecore.ExperienceEditor.Speak.Server.Contexts;
    using System;

    public class CheckRevision : PipelineProcessorRequest<PageContext>
    {
        public override PipelineProcessorResponseValue ProcessRequest()
        {
            SaveArgs.SaveItem item = base.RequestContext.GetSaveArgs().Items[0];
            PipelineProcessorResponseValue value2 = new PipelineProcessorResponseValue();
            Item item2 = base.RequestContext.Item.Database.GetItem(item.ID, Language.Parse(base.RequestContext.Language), Sitecore.Data.Version.Parse(base.RequestContext.Version));
            if (item2 != null)
            {
                string strA = item2[FieldIDs.Revision].Replace("-", string.Empty);
                if (item.Revision == string.Empty)
                {
                    item.Revision = strA;
                }
                string strB = item.Revision.Replace("-", string.Empty);
                if ((string.Compare(strA, strB, StringComparison.InvariantCultureIgnoreCase) != 0) && (string.Compare("#!#Ignore revision#!#", strB, StringComparison.InvariantCultureIgnoreCase) != 0))
                {
                    value2.ConfirmMessage = Translate.Text("One or more items have been changed.\n\nDo you want to overwrite these changes?");
                }
            }
            return value2;
        }
    }
}

