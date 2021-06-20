using Advokates_CRM_DTO.Outputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Advokates_CRM.BL.Helpers
{
    public static class ErrorHandler<T> where T : ResultBase
    {
        public static T SetDBProblem(T t, string text)
        {
            t.Status = ResultBase.StatusBad;
            t.ErrorMessages.Add(new ErrorMessageResult
                                    {
                                        message = text
                                    });
            return t;
        }

        public static ResultBase TokenNotValid()
        {
            ResultBase result = new ResultBase();
            result.Status = ResultBase.StatusBad;
            result.Status = ResultBase.StatusBad;
            result.ErrorMessages.Add(new ErrorMessageResult
                                        {
                                            message = "Token Invalid"
                                        });
            return result;
        }
    }
}
