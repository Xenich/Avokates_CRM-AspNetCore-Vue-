using Advokates_CRM_DTO.Outputs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Advokates_CRM.Layer_Interfaces
{
    public interface IErrorHandler
    {
        T SetDBProblem<T>(T t, string text) where T : ResultBase;
        ResultBase TokenNotValid();
    }
}

