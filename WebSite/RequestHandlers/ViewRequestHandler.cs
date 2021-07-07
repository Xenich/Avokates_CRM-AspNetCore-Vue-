using Advokates_CRM.Layer_Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Avokates_CRM.RequestHandlers
{
    public class ViewRequestHandler : Controller, IViewRequestHandler
    {
        public IActionResult HandleRequest(ViewLambdaDelegate viewLambda, IErrorHandler _errorHandler)
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
