using System.Diagnostics;
using Advokates_CRM_DTO.Outputs;
using Microsoft.AspNetCore.Mvc;
using Avokates_CRM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Advokates_CRM.Layer_Interfaces;
using Avokates_CRM.RequestHandlers;

namespace Avokates_CRM.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly IDataLayer dl;      // dataLayer
        private readonly IDataLayerEmployee _dlEmployee;
        private readonly IViewRequestHandler _viewRequestHandler;

        public HomeController(IDataLayer dataLayer, IDataLayerEmployee dlEmployee, IViewRequestHandler viewRequestHandler, IErrorHandler errorHandler)      // в Startup : AddScoped<IDataLayer, DataLayer>();
            : base(errorHandler)
        {
            dl = dataLayer;
            _dlEmployee = dlEmployee;
            _viewRequestHandler = viewRequestHandler;
        }

        public IActionResult Index()
        {
            ViewLambdaDelegate viewLambda = () =>
            {
                string token = GetToken();
                GetMainPage_Out result = dl.GetMainPage(token);
                return View(result);
            };
            return _viewRequestHandler.HandleRequest(viewLambda, _errorHandler);
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
            ViewLambdaDelegate viewLambda = () =>
            {
                InviteResult result = _dlEmployee.Invite(token);
                return View(result);
            };
            return _viewRequestHandler.HandleRequest(viewLambda, _errorHandler);
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
            ViewLambdaDelegate viewLambda = () =>
            {
                string token = GetToken();
                ViewData["id"] = id.ToString();
                ViewData["userUid"] = _dlEmployee.GetUserUidByJWT(token);
                return View();
            };
            return _viewRequestHandler.HandleRequest(viewLambda, _errorHandler);
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
