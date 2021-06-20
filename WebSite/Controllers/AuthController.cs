using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Advokates_CRM_DTO.Outputs;

using Advokates_CRM_DTO.Inputs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Avokates_CRM.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Net;
using Advokates_CRM.Layer_Interfaces;

namespace Avokates_CRM.Controllers
{
    public class AuthController : Controller
    {
        private readonly ISecurity dl;      // dataLayer
        public AuthController(ISecurity dataLayer)      // в Startup : AddScoped<ISecuryty, Security>();
        {
            dl = dataLayer;
        }

        [AllowAnonymous]
        public IActionResult Authorization()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Authorization(Authorization_In auth)
        {
            ResultBase res = dl.Authorization(auth);
            //string resp = Helpers.AuthHelper.Authorization(auth.Login, auth.Password);
            //AuthResponce res = Newtonsoft.Json.JsonConvert.DeserializeObject<AuthResponce>(resp);
            
            if (res.Status == "bad")
            {
                HttpContext.Session.Clear();
                ViewBag.error = "Неправильно введён логин или пароль!";
                return Ok(res);
            }

            else
            {
                HttpContext.Session.SetString("token", (res as Authorization_Out).Token);
                HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
               // return Ok(res);
               return Redirect("~/Home/Index");
            }
        }
    }
}