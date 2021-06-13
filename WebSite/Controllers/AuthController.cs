﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avokates_CRM.Models.Outputs;

using Avokates_CRM.Models.Inputs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Avokates_CRM.Models;
using WebSite.DataLayer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Avokates_CRM.Models.ApiModels;
using System.Net;

namespace Avokates_CRM.Controllers
{
    public class AuthController : Controller
    {
        private readonly ISecuryty dl;      // dataLayer
        public AuthController(ISecuryty dataLayer)      // в Startup : AddScoped<ISecuryty, Security>();
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