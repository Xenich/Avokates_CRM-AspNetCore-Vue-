using Advokates_CRM.Layer_Interfaces;
using Avokates_CRM.Controllers;
using Avokates_CRM.RequestHandlers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Avokates_CRM.RequestHandlers
{
    public interface IBasicRequestHandler
    {
        IActionResult HandleRequest(LambdaDelegate lambda, IErrorHandler _errorHandler);
    }
}
