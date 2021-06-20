using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Advokates_CRM.Layer_Interfaces;
using Advokates_CRM_DTO.Outputs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Avokates_CRM.Controllers
{

    public class BaseController : Controller
    {
        protected delegate ResultBase Lambda();
        protected delegate ViewResult ViewLambda();
        protected readonly IErrorHandler _errorHandler;

        public BaseController(IErrorHandler errorHandler)
        {
            _errorHandler = errorHandler;
        }

        protected string GetToken()
        {
            string b = Request.Headers["authorization"].ToString();
            return b.Substring(7, b.Length - 7);    // первые 7 символов - это слово "Bearer "
        }

        protected IActionResult HandleRequestBasic(Lambda lambda)
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

        protected IActionResult HandleRequestView(ViewLambda viewLambda)
        {
            try
            {
                ViewResult result = viewLambda();
                return result;
            }
            catch (Exception ex)
            {
                return View("ErrorView");

                //TODO : внедрить логгер ошибок
            }
        }
    }
}