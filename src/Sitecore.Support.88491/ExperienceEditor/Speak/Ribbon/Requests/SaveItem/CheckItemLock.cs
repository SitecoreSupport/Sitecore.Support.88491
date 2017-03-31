namespace Sitecore.Support.ExperienceEditor.Speak.Ribbon.Requests.SaveItem
{
  using Sitecore.Data;
  using Sitecore.ExperienceEditor.Speak.Server.Responses;
  using Sitecore.Pipelines;
  using Sitecore.Pipelines.Save;
  using Sitecore.Support.ExperienceEditor.Speak.Server.Requests;
  using Sitecore.ExperienceEditor.Speak.Server.Contexts;
  using Data.Items;
  using Globalization;

  public class CheckItemLock : PipelineProcessorRequest<PageContext>
  {
    public override PipelineProcessorResponseValue ProcessRequest()
    {
      base.RequestContext.ValidateContextItem();
      PipelineProcessorResponseValue value2 = new PipelineProcessorResponseValue();
      Item item = base.RequestContext.Item;
      if (item.Locking.IsLocked() && !item.Locking.HasLock())
      {
        value2.AbortMessage = Translate.Text("Unable to save changes because the corresponding content has been locked by another user.");
      }
      return value2;
    }
  }
}

