using Advokates_CRM.Layer_Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Avokates_CRM.RequestHandlers
{
    public interface IViewRequestHandler
    {
        IActionResult HandleRequest(ViewLambdaDelegate lambda, IErrorHandler _errorHandler);
    }
}
