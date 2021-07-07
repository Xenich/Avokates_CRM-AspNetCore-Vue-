using Advokates_CRM.Layer_Interfaces;
using Advokates_CRM_DTO.Outputs;
using Avokates_CRM.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;



namespace Avokates_CRM.RequestHandlers
{
    internal class BasicRequestHandler : Controller, IBasicRequestHandler
    {
        public IActionResult HandleRequest(LambdaDelegate lambda, IErrorHandler _errorHandler)
        {
            ResultBase result;
            try
            {
                result = lambda();
                if (string.IsNullOrEmpty(result.Status))
                    result.Status = ResultBase.StatusOk;
                return Ok(result);
            }
            catch (Exception ex)
            {
                result = new ResultBase();
                result.Status = ResultBase.StatusBad;
                return Ok(_errorHandler.SetDBProblem<ResultBase>(result, ex.Message));

                //TODO : внедрить логгер ошибок
            }
        }
    }
}
