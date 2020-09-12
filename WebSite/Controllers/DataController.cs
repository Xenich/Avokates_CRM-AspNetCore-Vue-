using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avokates_CRM.Models.Inputs;
using Avokates_CRM.Models.Outputs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using WebSite.DataLayer;
using WebSite.Helpers;

using Avokates_CRM.Models.Outputs;
using Microsoft.AspNetCore.Http;

namespace WebSite.Controllers
{
    [Authorize]
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

        [Authorize(Roles = "admin, director")]
        public IActionResult CreateInvite(string email)
        {          
            ResultBase result = dl.CreateInvite(GetToken(), email);
            return Ok(result);
        }

        // Создание нового юзера по пригласительному токену
        [AllowAnonymous]
        public IActionResult CreateUserByInvite(Registration_In value)
        {
            Registration_Out result = dl.CreateUserByInvite(value);
            if (result.Status == ResultBase.StatusOk)
            {
                HttpContext.Session.SetString("token", result.JWT);
            }
            return Ok(result);
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

        public IActionResult NewCaseGetModel()
        {
            NewCaseGetModel_Out result = dl.NewCaseGetModel();
            return Ok(result);
        }

        public IActionResult CreateNewCase(NewCase_In value)
        {
            value.Token = GetToken();
            ResultBase result = dl.CreateNewCase(value);
            return Ok(result);
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