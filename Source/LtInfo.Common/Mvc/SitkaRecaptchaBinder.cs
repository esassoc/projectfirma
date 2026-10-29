/*-----------------------------------------------------------------------
<copyright file="SitkaRecaptchaBinder.cs" company="Environmental Science Associates">
Copyright (c) Environmental Science Associates. All rights reserved.
<author>Environmental Science Associates</author>
</copyright>

<license>
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License <http://www.gnu.org/licenses/> for more details.

Source code is available upon request via <support@sitkatech.com>.
</license>
-----------------------------------------------------------------------*/
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using LtInfo.Common.Interfaces;

namespace LtInfo.Common.Mvc
{
    public class SitkaRecaptchaBinder<T> : DefaultModelBinder where T : class, IRecaptchaModel, new()
    {
        private readonly string _privateKey;
        private readonly string _validationUrl;
        private readonly bool _validateOnBind;
        private readonly Action<string> _loggingFunction;


        public SitkaRecaptchaBinder(string privateKey, string validatorUrl, bool validateOnBind,  Action<string> loggingFunction)
        {
            _privateKey = privateKey;
            _validationUrl = validatorUrl;
            _validateOnBind = validateOnBind;
            _loggingFunction = loggingFunction;
        }

        public SitkaRecaptchaBinder(string privateKey, string validatorUrl, bool validatOnBind)
            : this(privateKey, validatorUrl, validatOnBind, SitkaLogger.Instance.LogDetailedWarningMessage)
        {            
        }

        public SitkaRecaptchaBinder(string privateKey, string validatorUrl)
            :this(privateKey, validatorUrl, true)
        {            
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            IRecaptchaModel viewModel = base.BindModel(controllerContext, bindingContext) as IRecaptchaModel;

            if(viewModel == null)
            {
                throw new ArgumentException("Unable to bind RecaptchaModel");
            }

            viewModel.ClientIP = controllerContext.HttpContext.Request.UserHostAddress;
            viewModel.PrivateKey = _privateKey;
            viewModel.ValidationUrl = _validationUrl;

            if(_validateOnBind)
            {
                viewModel.IsValid = IsValidResponse(viewModel);
                if(!viewModel.IsValid)
                {
                    bindingContext.ModelState.AddModelError("IRecaptchaModel", "Invalid Recaptcha Response");
                }
            }
            
            return viewModel;
        }

        private bool IsValidResponse(IRecaptchaModel viewModel)
        {
            // bool IsValidResponse(string challenge, string response, string clientIP, string privateKey, string validationUrl, Action<string> loggingFunction)
            if (viewModel.recaptcha_challenge_field == null || viewModel.recaptcha_response_field == null || viewModel.ClientIP == null)
            {
                _loggingFunction(string.Format("Problem with Captcha. One or more of the required Captcha parts is null (challenge, response, or clientIP). Asking client to re-try. If this happens repeatedly it may indicate a change to the Captcha API. (did not cause a user visible exception)\r\nchallenge: {0}\r\nresponse: {1}\r\nclientIP: {2}.", 
                                               viewModel.recaptcha_challenge_field, 
                                               viewModel.recaptcha_response_field, 
                                               viewModel.ClientIP));
                return false;
            }

            var request = (HttpWebRequest)WebRequest.Create(viewModel.ValidationUrl);
            request.ProtocolVersion = HttpVersion.Version10;
            request.Timeout = 30 * 1000;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            string formdata = string.Format("privatekey={0}&remoteip={1}&challenge={2}&response={3}"
                                            , new object[]
                                                  {
                                                      HttpUtility.UrlEncode(viewModel.PrivateKey)
                                                      ,   HttpUtility.UrlEncode(viewModel.ClientIP)
                                                      ,   HttpUtility.UrlEncode(viewModel.recaptcha_challenge_field)
                                                      ,   HttpUtility.UrlEncode(viewModel.recaptcha_response_field)
                                                  });

            byte[] formbytes = Encoding.ASCII.GetBytes(formdata);

            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(formbytes, 0, formbytes.Length);
            }

            using (var httpResponse = request.GetResponse())
            {
                Stream stream = httpResponse.GetResponseStream();
                if(stream != null)
                {
                    using (var readStream = new StreamReader(stream, Encoding.UTF8))
                    {
                        string[] results = readStream.ReadToEnd().Split(new char[0]);

                        switch (results[0])
                        {
                            case "true":
                                return true;

                            case "false":
                                return false;
                            default:
                                throw new InvalidProgramException(string.Format("Unknown status response \"{0}\" from Recaptcha validation call.", results[0]));
                        }
                    }    
                }                
            }
            return false;
        }
    }
}
