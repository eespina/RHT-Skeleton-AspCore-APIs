using Microsoft.AspNetCore.Mvc.ModelBinding;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Extensions
{
    public interface IErrorHandler
    {
        Task<string> GetErrorMessage(ModelStateDictionary modelState);
    }

    public class ErrorHandler : IErrorHandler
    {
        public Logger logger = LogManager.GetCurrentClassLogger();

        public async Task<string> GetErrorMessage(ModelStateDictionary modelState)
        {
            try
            {
                logger.Trace($"inside {MethodBase.GetCurrentMethod().Name}.");
                var errorList = new List<string>();

                foreach (var key in modelState.Keys)
                {
                    var modelStateVal = modelState[key];
                    var messageList = new List<string>(modelStateVal.Errors.Select(x => x.ErrorMessage).ToList());
                    var message = string.Join(", ", messageList);
                    errorList.Add($"{key} : {message}");
                }

                if (errorList.Count > 0)
                {
                    var errorString = string.Join(". ", errorList);
                    return errorString;
                }
                else
                {
                    logger.Warn("Invalid Model Detected, but no invalidations were recorded.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                logger.Error("ERROR Creating Error Messages (Error Message back will be NULL):" + ex);
                return null;
            }
        }
    }
}
