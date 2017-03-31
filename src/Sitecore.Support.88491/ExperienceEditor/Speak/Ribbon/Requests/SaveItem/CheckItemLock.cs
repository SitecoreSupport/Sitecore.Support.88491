﻿namespace Sitecore.Support.ExperienceEditor.Speak.Ribbon.Requests.SaveItem
{
    using Sitecore.Globalization;
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using Sitecore.ExperienceEditor.Speak.Server.Responses;
    using Sitecore.ExperienceEditor.Switchers;
    using Sitecore.Pipelines;
    using Sitecore.Pipelines.Save;
    using Sitecore.Support.ExperienceEditor.Speak.Server.Requests;
    using Sitecore.ExperienceEditor.Speak.Server.Contexts;

    public class CheckItemLock : PipelineProcessorRequest<PageContext>
    {
        public override PipelineProcessorResponseValue ProcessRequest()
        {
            base.RequestContext.ValidateContextItem();
            PipelineProcessorResponseValue pipelineProcessorResponseValue = new PipelineProcessorResponseValue();
            Item item = base.RequestContext.Item;
            if (item.Locking.IsLocked() && !item.Locking.HasLock())
            {
                pipelineProcessorResponseValue.AbortMessage = Translate.Text("Unable to save changes because the corresponding content has been locked by another user.");
            }
            return pipelineProcessorResponseValue;
        }
    }
}
