using System;
using System.Collections.Generic;
using System.Text;
using Advokates_CRM.Layer_Interfaces;
using Advokates_CRM_DTO.Outputs;

namespace Advokates_CRM.BL
{
    public class DataLayerError :IErrorHandler
    {
        public  T SetDBProblem<T>(T result, string text) where T : ResultBase
        {
            result.Status = ResultBase.StatusBad;
            result.ErrorMessages.Add(new ErrorMessageResult { message = text });
            return result;
        }

        public ResultBase TokenNotValid()
        {
            ResultBase result = new ResultBase();
            result.Status = ResultBase.StatusBad;
            result.ErrorMessages.Add(new ErrorMessageResult { message = "Token Invalid" });
            return result;
        }
    }
}
