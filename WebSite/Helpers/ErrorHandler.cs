using Avokates_CRM.Models.Outputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebSite.Helpers
{
    public static class ErrorHandler<T> where T : ResultBase
    {
        public static T SetDBProblem(T t, string text)
        {
            t.Status = ResultBase.StatusBad;
            t.ErrorMessages = new List<ErrorMessageResult>
            {
                new ErrorMessageResult
                {
                    message = text
                }
            };
            return t;
        }

        public static ResultBase TokenNotValid()
        {
            ResultBase result = new ResultBase();
            result.Status = ResultBase.StatusBad;
            result.ErrorMessages = new List<ErrorMessageResult>()
            {
                new ErrorMessageResult
                {
                    message = "Token Invalid"
                }
            };
            return result;
        }
    }
}
