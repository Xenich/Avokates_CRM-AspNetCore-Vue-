using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using WebSite.DataLayer;
using WebSite.Helpers;
using WebSite.Models.Inputs;
using WebSite.Models.Outputs;

namespace WebSite.Controllers
{
    public class DataController : Controller
    {
        private readonly IDataLayer dl;      // dataLayer
        public DataController(IDataLayer dataLayer)      // в Startup : AddScoped<IDataLayer, DataLayer>();
        {
            dl = dataLayer;
        }

        public IActionResult Test([FromBody] BaseAuth_In auth)
        {
            var c = HttpContext.Request;
            if (HelperSecurity.IsTokenValid(auth.Token))
            {
                GetCasesList_Out result = dl.GetCasesList(auth.Token);
                return Ok(result);
            }
            else
                return Ok(ErrorHandler<ResultBase>.TokenNotValid());

        }

        public IActionResult GetCasesList(String foo)
        {
            //var c = HttpContext.Request;
            //if (HelperSecurity.IsTokenValid(token))
            //{
                GetCasesList_Out result = dl.GetCasesList(GetToken());
                return Ok(result);
            //}
            //else
            //    return Ok(ErrorHandler<ResultBase>.TokenNotValid());

        }

        private string GetToken()
        {
            string b = Request.Headers["authorization"].ToString();
            return b.Substring(7, b.Length - 7);    // первые 7 символов - это слово bearer_            
        }
    }
}