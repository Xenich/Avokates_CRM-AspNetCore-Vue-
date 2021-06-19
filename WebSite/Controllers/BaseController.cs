using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Avokates_CRM.Controllers
{

    public class BaseController : Controller
    {
        protected string GetToken()
        {
            string b = Request.Headers["authorization"].ToString();
            return b.Substring(7, b.Length - 7);    // первые 7 символов - это слово "Bearer "
        }
    }
}