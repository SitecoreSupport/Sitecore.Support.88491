namespace Sitecore.Support.ExperienceEditor.Speak.Ribbon.Requests.SaveItem
{
    using Sitecore.Data;
    using Sitecore.Data.Fields;
    using Sitecore.Data.Items;
    using Sitecore.ExperienceEditor.Speak.Server.Responses;
    using Sitecore.Globalization;
    using Sitecore.Pipelines.Save;
    using Sitecore.Support.ExperienceEditor.Speak.Server.Requests;
    using Sitecore.ExperienceEditor.Speak.Server.Contexts;

    public class ValidateFields : PipelineProcessorRequest<PageContext>
    {
        public override PipelineProcessorResponseValue ProcessRequest()
        {
            PipelineProcessorResponseValue value2 = new PipelineProcessorResponseValue();
            SaveArgs.SaveItem item = base.RequestContext.GetSaveArgs().Items[0];
            Item item2 = base.RequestContext.Item.Database.GetItem(item.ID, item.Language);
            if (((item2 != null) && !item2.Paths.IsMasterPart) && !StandardValuesManager.IsStandardValuesHolder(item2))
            {
                foreach (SaveArgs.SaveField field in item.Fields)
                {
                    Field field2 = item2.Fields[field.ID];
                    string fieldRegexValidationError = FieldUtil.GetFieldRegexValidationError(field2, field.Value);
                    if (!string.IsNullOrEmpty(fieldRegexValidationError))
                    {
                        value2.AbortMessage = Translate.Text(fieldRegexValidationError);
                        return value2;
                    }
                }
            }
            return value2;
        }
    }
}

