namespace Sitecore.Support.ExperienceEditor.Speak.Ribbon.Requests.SaveItem
{
  using Sitecore.Collections;
  using Sitecore.Configuration;
  using Sitecore.Data;
  using Sitecore.Data.Items;
  using Sitecore.Data.Validators;
  using Sitecore.Diagnostics;
  using Sitecore.ExperienceEditor.Speak.Server.Responses;
  using Sitecore.ExperienceEditor.Utils;
  using Sitecore.Globalization;
  using Sitecore.Support.ExperienceEditor.Speak.Server.Requests;
  using Sitecore.ExperienceEditor.Speak.Server.Contexts;
  using System.Collections.Generic;
  using System.Text;

  public class Validators : PipelineProcessorRequest<PageContext>
  {
    private static string GetValidationErrorDetails(BaseValidator failedValidator)
    {
      Assert.ArgumentNotNull(failedValidator, "failedValidator");
      if (failedValidator.IsValid)
      {
        return string.Empty;
      }
      StringBuilder builder = new StringBuilder();
      if (!string.IsNullOrEmpty(failedValidator.Text))
      {
        builder.AppendLine("\n\n" + failedValidator.Text);
      }
      foreach (string str in failedValidator.Errors)
      {
        if (!string.IsNullOrEmpty(str))
        {
          builder.AppendLine(" - " + str);
        }
      }
      return builder.ToString();

    }

    public override PipelineProcessorResponseValue ProcessRequest()
    {
      PipelineProcessorResponseValue value2 = new PipelineProcessorResponseValue();
      string validatorsKey = base.RequestContext.ValidatorsKey;
      if (!string.IsNullOrEmpty(validatorsKey))
      {
        ValidatorCollection validators = ValidatorManager.GetValidators(ValidatorsMode.ValidatorBar, validatorsKey);
        ValidatorOptions options = new ValidatorOptions(true);
        ValidatorManager.Validate(validators, options);
        ValidatorResult valid = ValidatorResult.Valid;
        BaseValidator failedValidator = null;
        foreach (BaseValidator validator2 in validators)
        {
          ValidatorResult result = validator2.Result;
          if (validator2.ItemUri != null)
          {
            Item item = base.RequestContext.Item.Database.GetItem(validator2.ItemUri.ToDataUri());
            if (((item != null) && StandardValuesManager.IsStandardValuesHolder(item)) && (result > ValidatorResult.CriticalError))
            {
              result = ValidatorResult.CriticalError;
            }
          }
          if (result > valid)
          {
            valid = result;
            failedValidator = validator2;
          }
          if (validator2.IsEvaluating && (validator2.MaxValidatorResult >= ValidatorResult.CriticalError))
          {
            value2.AbortMessage = Translate.Text("The fields in this item have not been validated.\n\nWait until validation has been completed and then save your changes.");
            return value2;
          }
        }
        switch (valid)
        {
          case ValidatorResult.CriticalError:
            {
              string key = Translate.Text("Some of the fields in this item contain critical errors.\n\nAre you sure you want to save this item?");
              if (failedValidator != null)
              {
                key = key + GetValidationErrorDetails(failedValidator);
              }
              value2.ConfirmMessage = Translate.Text(key);
              return value2;
            }
          case ValidatorResult.FatalError:
            {
              string str3 = Translate.Text("Some of the fields in this item contain fatal errors.\n\nYou must resolve these errors before you can save this item.");
              if (failedValidator != null)
              {
                str3 = str3 + GetValidationErrorDetails(failedValidator);
              }
              value2.ConfirmMessage = Translate.Text(str3);
              break;
            }
        }
      }
      return value2;
    }
  }
}

