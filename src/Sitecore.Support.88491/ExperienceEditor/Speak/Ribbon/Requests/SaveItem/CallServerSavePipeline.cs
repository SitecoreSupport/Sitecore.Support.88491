namespace Sitecore.Support.ExperienceEditor.Speak.Ribbon.Requests.SaveItem
{
  using Sitecore.Caching;
  using Sitecore.Data;
  using Sitecore.ExperienceEditor.Speak.Server.Responses;
  using Sitecore.Globalization;
  using Sitecore.Pipelines;
  using Sitecore.Pipelines.Save;
  using Sitecore.ExperienceEditor.Speak.Server.Contexts;
  using Sitecore.Support.ExperienceEditor.Speak.Server.Requests;

  public class CallServerSavePipeline : PipelineProcessorRequest<PageContext>
  {
    public override PipelineProcessorResponseValue ProcessRequest()
    {
      PipelineProcessorResponseValue value2 = new PipelineProcessorResponseValue();
      Pipeline pipeline = PipelineFactory.GetPipeline("saveUI");
      pipeline.ID = ShortID.Encode(ID.NewID);
      SaveArgs saveArgs = base.RequestContext.GetSaveArgs();
      saveArgs.CustomData["showvalidationdetails"] = true;
      if (Sitecore.Context.Items["sc_ContentDatabase"] == null)
      {
        Sitecore.Context.Items["sc_ContentDatabase"] = base.RequestContext.Item.Database;
      }
      pipeline.Start(saveArgs);
      CacheManager.GetItemCache(base.RequestContext.Item.Database).Clear();
      value2.AbortMessage = Translate.Text(saveArgs.Error);
      return value2;
    }
  }
}

