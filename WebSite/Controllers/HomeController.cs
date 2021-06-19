﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Avokates_CRM.Models.Outputs;
using Microsoft.AspNetCore.Mvc;

using Avokates_CRM.Models;
using Avokates_CRM.Models.ApiModels;
using WebSite.DataLayer;
using WebSite.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Avokates_CRM.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly IDataLayer dl;      // dataLayer
        public HomeController(IDataLayer dataLayer)      // в Startup : AddScoped<IDataLayer, DataLayer>();
        {
            dl = dataLayer;
        }

        public IActionResult Index()
        {
            string token = GetToken();
            if (HelperSecurity.IsTokenValid(token))
            {
                GetMainPage_Out result = dl.GetMainPage(token);
                return View(result);
            }
            else
                return View("ErrorView");
        }

        public IActionResult UnLogin()
        {
            HttpContext.Session.Clear();
            return Redirect("~/Auth/Authorization");

        }


        [AllowAnonymous]
        [HttpGet]
        public IActionResult Invite(string token)
        {
            InviteResult result = dl.Invite(token);
            return View(result);
        }

        public IActionResult Cabinet()
        {
            return View();
        }

        [Authorize(Roles = "admin, director")]
        [HttpGet]
        public IActionResult Employees()
        {
            return View();
        }

            // получение определенного дела
        public IActionResult Case(int id)
        {
            string token = GetToken();
            ViewData["id"] = id.ToString();
            ViewData["userUid"] = HelperSecurity.GetUserUidByJWT(token).ToString();               
            return View();
        }

        public IActionResult NewCase()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Test()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
