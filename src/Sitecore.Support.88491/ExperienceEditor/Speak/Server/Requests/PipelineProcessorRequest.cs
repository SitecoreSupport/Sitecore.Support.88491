namespace Sitecore.Support.ExperienceEditor.Speak.Server.Requests
{
    using Sitecore;
    using Sitecore.Configuration;
    using Sitecore.Diagnostics;
    using Sitecore.Exceptions;
    using Sitecore.ExperienceEditor.Exceptions;
    using Sitecore.ExperienceEditor.Speak.Server;
    using Sitecore.ExperienceEditor.Speak.Server.Responses;
    using Sitecore.ExperienceEditor.Utils;
    using Sitecore.Globalization;
    using Sitecore.Publishing;
    using Sitecore.Security.Accounts;
    using System;

    public abstract class PipelineProcessorRequest<T> : Sitecore.ExperienceEditor.Speak.Server.Requests.Request where T: Sitecore.ExperienceEditor.Speak.Server.Contexts.Context
    {
        protected PipelineProcessorRequest()
        {
        }

        protected Response GenerateExceptionResponse(string errorMessage, Exception exception)
        {
            Log.Error(exception.Message, exception, this);
            return new Response { 
                Error = true,
                ErrorMessage = errorMessage
            };
        }

        [Obsolete("This method is obsolete and will be removed in the next product version. Use GenerateExceptionResponse(string errorMessage, Exception exception, string postScriptFunc) instead.")]
        protected Response GenerateExceptionResponse(string errorMessage, string logMessage)
        {
            Log.Error(logMessage, this);
            return new Response { 
                Error = true,
                ErrorMessage = errorMessage
            };
        }

        protected User GetUser()
        {
            User user = Context.User;
            if (!(((user.Name == @"extranet\Anonymous") && Context.PageMode.IsPreview) && Settings.Preview.AsAnonymous))
            {
                return user;
            }
            string shellUser = PreviewManager.GetShellUser();
            if (string.IsNullOrEmpty(shellUser))
            {
                return user;
            }
            return User.FromName(shellUser, true);
        }

        public override Response Process(RequestArgs requestArgs)
        {
            Response response;
            LanguageSwitcher switcher2;
            try
            {
                Assert.ArgumentNotNull(requestArgs, "requestArgs");
                this.Args = requestArgs;
                this.RequestContext = this.GetContext<T>(this.Args.Data);
                Assert.IsNotNull(this.RequestContext, "Could not get context for requestArgs:{0}", new object[] { requestArgs.Data });
                using (new UserSwitcher(this.GetUser()))
                {
                    using (switcher2 = new LanguageSwitcher(WebUtility.ClientLanguage))
                    {
                        return new PipelineProcessorResponse { 
                            Error = false,
                            ErrorMessage = null,
                            ResponseValue = this.ProcessRequest()
                        };
                    }
                }
            }
            catch (FieldValidationException exception)
            {                
                response = this.GenerateExceptionResponse(exception.Message, exception);
            }
            catch (ItemNotFoundException exception2)
            {
                using (switcher2 = new LanguageSwitcher(WebUtility.ClientLanguage))
                {
                    response = this.GenerateExceptionResponse(Translate.Text("The item does not exist. It may have been deleted by another user."), exception2);
                }
            }
            catch (Exception exception3)
            {
                response = this.GenerateExceptionResponse(Translate.Text("MY:  An error occurred."), exception3);
            }
            return response;
        }

        public abstract PipelineProcessorResponseValue ProcessRequest();

        public RequestArgs Args { get; private set; }

        public T RequestContext { get; set; }
    }
}

