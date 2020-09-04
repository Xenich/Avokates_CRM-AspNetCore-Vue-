using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avokates_CRM.Models.Inputs;
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

        [HttpPost]
        public IActionResult CreateInvite([FromBody] CreateInvite_In value)
        {
            string token = GetToken();
            //token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbXBsb3llZVVpZCI6IjcxYzkwM2IwLWE0ZDQtZWExMS1hMTQ0LWUwZDU1ZTQ0OTkxMCIsImxvZ2luIjoibG9naW4iLCJjb21wYW55VWlkIjoiYjliMjBhMTgtYTNkNC1lYTExLWExNDQtZTBkNTVlNDQ5OTEwIiwidXNlck5hbWUiOiLQmNCy0LDQvSIsInJvbGVOYW1lIjoiZGlyZWN0b3IiLCJuYmYiOjE1OTkyNDEzNzIsImV4cCI6MTU5OTI0NDk3MiwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIvIiwiYXVkIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzIvIn0.AugeLImZm-lHbnVx_y-UX4xavMxaG5bD9sjQVJgyeWo";
            if (HelperSecurity.IsTokenValid(token))
            {
                ResultBase result = dl.CreateInvite(token, value.Email);
                return Ok(result);
            }
            else
                return Ok(ErrorHandler<ResultBase>.TokenNotValid());
        }

        public IActionResult GetCasesList()
        {            
            string token = GetToken();
            if (HelperSecurity.IsTokenValid(token))
            {
                GetCasesList_Out result = dl.GetCasesList(token);
                return Ok(result);
            }
            else
                return Ok(ErrorHandler<ResultBase>.TokenNotValid());
        }

        public IActionResult GetMainPage()
        {
            string token = GetToken();
            if (HelperSecurity.IsTokenValid(token))
            {
                GetMainPage_Out result = dl.GetMainPage(GetToken());
                return Ok(result);
            }
            else
                return Ok(ErrorHandler<ResultBase>.TokenNotValid());
        }

        public IActionResult GetCaseInfo(int id)
        {
            string token = GetToken();
            if (HelperSecurity.IsTokenValid(token))
            {
                GetCase_Out result = dl.GetCase(token, id);
                return Ok(result);
            }
            else
                return Ok(ErrorHandler<ResultBase>.TokenNotValid());
        }

        public IActionResult GetCabinetInfo()
        {
            string token = GetToken();
            if (HelperSecurity.IsTokenValid(token))
            {
                GetCabinetInfo_Out result  = dl.GetCabinetInfo(token);
                return Ok(result);
                //return Ok(ErrorHandler<ResultBase>.TokenNotValid());
                
                //GetCase_Out result 
                //return Ok(result);
            }
            else
                return Ok(ErrorHandler<ResultBase>.TokenNotValid());
        }

        public IActionResult CabinetInfoSaveChanges(GetCabinetInfo_Out cabinetInfo)
        {
            string token = GetToken();
            if (HelperSecurity.IsTokenValid(token))
            {
                ResultBase result = dl.CabinetInfoSaveChanges(token, cabinetInfo);
                return Ok(result);
            }
            else
                return Ok(ErrorHandler<ResultBase>.TokenNotValid());
        }

        private string GetToken()
        {
            string b = Request.Headers["authorization"].ToString();
            return b.Substring(7, b.Length - 7);    // первые 7 символов - это слово bearer_            
        }
    }
}