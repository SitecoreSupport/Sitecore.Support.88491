namespace Sitecore.Support.ExperienceEditor.Speak.Ribbon.Requests.SaveItem
{
    using Sitecore.Collections;
    using Sitecore.Configuration;
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using Sitecore.Data.Validators;
    using Sitecore.Diagnostics;
    using Sitecore.ExperienceEditor.Speak.Server.Responses;
    using Sitecore.ExperienceEditor.Switchers;
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

        protected virtual IEnumerable<BaseValidator> GetValidators(Item item)
        {
            ValidatorsMode mode;
            SafeDictionary<FieldDescriptor, string> controlsToValidate = base.RequestContext.GetControlsToValidate();
            ValidatorCollection validators = PipelineUtil.GetValidators(item, controlsToValidate, out mode);
            validators.Key = base.RequestContext.ValidatorsKey;
            return validators;
        }

        public override PipelineProcessorResponseValue ProcessRequest()
        {
            Item item = base.RequestContext.Item;
            PipelineProcessorResponseValue value2 = new PipelineProcessorResponseValue();
            if (Settings.WebEdit.ValidationEnabled)
            {
                Pair<ValidatorResult, BaseValidator> pair;
                if (string.IsNullOrEmpty(base.RequestContext.ValidatorsKey))
                {
                    return value2;
                }
                using (new ClientDatabaseSwitcher(item.Database))
                {
                    ValidatorCollection validators = this.GetValidators(item) as ValidatorCollection;
                    ValidatorOptions options = new ValidatorOptions(true);
                    ValidatorManager.Validate(validators, options);
                    pair = ValidatorManager.GetStrongestResult(validators, true, true);
                }
                ValidatorResult result = pair.Part1;
                BaseValidator failedValidator = pair.Part2;
                if ((failedValidator != null) && failedValidator.IsEvaluating)
                {
                    value2.AbortMessage = Translate.Text("The fields in this item have not been validated.\n\nWait until validation has been completed and then save your changes.");
                    return value2;
                }
                switch (result)
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
                        string str2 = Translate.Text("Some of the fields in this item contain fatal errors.\n\nYou must resolve these errors before you can save this item.");
                        if (failedValidator != null)
                        {
                            str2 = str2 + GetValidationErrorDetails(failedValidator);
                        }
                        value2.AbortMessage = Translate.Text(str2);
                        return value2;
                    }
                }
            }
            return value2;
        }
    }
}

