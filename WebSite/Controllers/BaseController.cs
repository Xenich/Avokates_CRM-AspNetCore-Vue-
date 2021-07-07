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
    }
}