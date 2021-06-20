using System;
using System.Collections.Generic;
using System.Text;
using Advokates_CRM.Layer_Interfaces;
using Advokates_CRM_DTO.Outputs;

namespace Advokates_CRM.BL
{
    public class _ErrorHandler :IErrorHandler
    {
        public  T SetDBProblem<T>(T t, string text) where T : ResultBase
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

        public ResultBase TokenNotValid()
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
