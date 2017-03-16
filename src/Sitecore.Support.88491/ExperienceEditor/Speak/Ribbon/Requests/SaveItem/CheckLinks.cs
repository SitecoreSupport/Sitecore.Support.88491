namespace Sitecore.Support.ExperienceEditor.Speak.Ribbon.Requests.SaveItem
{
    using Sitecore.Data;
    using Sitecore.Data.Fields;
    using Sitecore.Data.Items;
    using Sitecore.ExperienceEditor.Speak.Server.Responses;
    using Sitecore.Globalization;
    using Sitecore.Links;
    using Sitecore.Support.ExperienceEditor.Speak.Server.Requests;
    using Sitecore.ExperienceEditor.Speak.Server.Contexts;
    using System.Text;

    public class CheckLinks : PipelineProcessorRequest<PageContext>
    {
        public override PipelineProcessorResponseValue ProcessRequest()
        {
            PipelineProcessorResponseValue value2 = new PipelineProcessorResponseValue();
            Item item = base.RequestContext.Item.Database.GetItem(base.RequestContext.ItemId, Language.Parse(base.RequestContext.Language), Sitecore.Data.Version.Parse(base.RequestContext.Version));
            if (item != null)
            {
                ItemLink[] brokenLinks = item.Links.GetBrokenLinks(false);
                if (brokenLinks.Length <= 0)
                {
                    return value2;
                }
                StringBuilder builder = new StringBuilder(Translate.Text("The item \"{0}\" contains broken links in these fields:\n\n", new object[] { item.DisplayName }));
                bool flag = false;
                foreach (ItemLink link in brokenLinks)
                {
                    if (!link.SourceFieldID.IsNull)
                    {
                        Field field = item.Fields[link.SourceFieldID];
                        builder.Append(" - ");
                        builder.Append((field != null) ? field.DisplayName : Translate.Text("[Unknown field: {0}]", new object[] { link.SourceFieldID.ToString() }));
                        if (!(string.IsNullOrEmpty(link.TargetPath) || ID.IsID(link.TargetPath)))
                        {
                            builder.Append(": \"");
                            builder.Append(link.TargetPath);
                            builder.Append("\"");
                        }
                        builder.Append("\n");
                    }
                    else
                    {
                        flag = true;
                    }
                }
                if (flag)
                {
                    builder.Append("\n");
                    builder.Append(Translate.Text("The template or branch for this item is missing."));
                }
                builder.Append("\n");
                builder.Append(Translate.Text("Do you want to save anyway?"));
                value2.ConfirmMessage = builder.ToString();
            }
            return value2;
        }
    }
}

