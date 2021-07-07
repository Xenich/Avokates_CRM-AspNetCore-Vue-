using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Advokates_CRM_DTO.Inputs;
using Advokates_CRM_DTO.Outputs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Advokates_CRM.Layer_Interfaces;
using Avokates_CRM.RequestHandlers;

namespace Avokates_CRM.Controllers
{
    [Authorize]
    public class DataController : BaseController
    {
        private readonly IDataLayer _dl;      // dataLayer
        private readonly IDataLayerCabinet _dlCabinet;
        private readonly IDataLayerCase _dlCase;
        private readonly IDataLayerNote _dlNote;
        private readonly IDataLayerFigurant _dlFigurant;
        private readonly IDataLayerEmployee _dlEmployee;

        private readonly IBasicRequestHandler _basicRequestHandler;



        public DataController(IDataLayer dataLayer, IDataLayerCabinet dlCabinet, IDataLayerCase dlCase,
                                IDataLayerNote dlNote, IDataLayerFigurant dlFigurant, IDataLayerEmployee dlEmployee,
                                IErrorHandler errorHandler, IBasicRequestHandler basicRequestHandler) : base(errorHandler)      // в Startup : AddScoped<IDataLayer, DataLayer>();
        {
            _dl = dataLayer;
            _dlCabinet = dlCabinet;
            _dlCase = dlCase;
            _dlNote = dlNote;
            _dlFigurant = dlFigurant;
            _dlEmployee = dlEmployee;
            _basicRequestHandler = basicRequestHandler;
        }

        public IActionResult Test([FromBody] BaseAuth_In auth)
        {
            GetCasesList_Out result = _dl.GetCasesList(auth.Token);
            return Ok(result);
        }

        [Authorize(Roles = "admin, director")]
        public IActionResult CreateInvite(string email)
        {
            LambdaDelegate lambda = () =>
            {
                ResultBase result = _dlEmployee.CreateInvite(GetToken(), email);
                return result;
            };
            return _basicRequestHandler.HandleRequest(lambda, _errorHandler);
        }

        // Создание нового юзера по пригласительному токену
        [AllowAnonymous]
        public IActionResult CreateUserByInvite(Registration_In value)
        {
            LambdaDelegate lambda = () =>
            {
                Registration_Out result = _dlEmployee.CreateUserByInvite(value);
                if (result.Status == ResultBase.StatusOk)
                {
                    HttpContext.Session.SetString("token", result.JWT);
                }
                return result;
            };
            return _basicRequestHandler.HandleRequest(lambda, _errorHandler);
        }


        public IActionResult GetMainPage()
        {
            LambdaDelegate lambda = () =>
            {
                string token = GetToken();
                GetMainPage_Out result = _dl.GetMainPage(token);
                return result;
            };
            return _basicRequestHandler.HandleRequest(lambda, _errorHandler);
        }

        public IActionResult GetCasesList()
        {
            LambdaDelegate lambda = () =>
            {
                string token = GetToken();
                GetCasesList_Out result = _dl.GetCasesList(token);
                return result;
            };
            return _basicRequestHandler.HandleRequest(lambda, _errorHandler);
        }
        
//----------------------------------------------    Личный кабинет      -----------------------------------------------------------------------------------

        #region CABINET

        public IActionResult GetCabinetInfo()
        {
            LambdaDelegate lambda = ()=>
            {
                string token = GetToken();
                GetCabinetInfo_Out result = _dlCabinet.GetCabinetInfo(token);
                return result;
            };
            return _basicRequestHandler.HandleRequest(lambda, _errorHandler);
        }

        public IActionResult CabinetInfoSaveChanges(CabinetInfo_In cabinetInfo)
        {
            LambdaDelegate lambda = () =>
            {
                string token = GetToken();
                ResultBase result = _dlCabinet.CabinetInfoSaveChanges(token, cabinetInfo);
                return result;
            };
            return _basicRequestHandler.HandleRequest(lambda, _errorHandler);
        }

        #endregion

//----------------------------------------------    ДЕЛО    -------------------------------------------------------

        #region CASE

        public IActionResult CreateNewCase(NewCase_In value)
        {
            LambdaDelegate lambda = () =>
            {
                value.Token = GetToken();
                ResultBase result = _dlCase.CreateNewCase(value);
                return result;
            };
            return _basicRequestHandler.HandleRequest(lambda, _errorHandler);
        }

        public IActionResult NewCaseGetModel()
        {
            LambdaDelegate lambda = () =>
            {
                string token = GetToken();
                NewCaseGetModel_Out result = _dlCase.NewCaseGetModel(token);
                return result;
            };
            return _basicRequestHandler.HandleRequest(lambda, _errorHandler);
        }

            // метод вызывается при обновлении списка записей (например при добавлении новой записи)
        public IActionResult GetCaseNotes(Guid caseUid, string privateKey, int elementsCount = 10, int currentPage = 1)
        {
            LambdaDelegate lambda = () =>
            {
                string token = GetToken();
                GetCaseNotes_Out result = _dlCase.GetCaseNotes(token, caseUid, privateKey, elementsCount, currentPage);
                return result;
            };
            return _basicRequestHandler.HandleRequest(lambda, _errorHandler);
        }

        public IActionResult GetCaseInfo(int id, string privateKey, int elementsCount = 10, int currentPage = 1)
        {
            LambdaDelegate lambda = () =>
            {
                string token = GetToken();
                GetCase_Out result = _dlCase.GetCase(token, id, privateKey, elementsCount, currentPage);
                return result;
            };
            return _basicRequestHandler.HandleRequest(lambda, _errorHandler);
        }

        public IActionResult GrantAccessToCase(Guid userUid, Guid caseUid, string privateKey)
        {
            LambdaDelegate lambda = () =>
            {
                string token = GetToken();
                ResultBase result = _dlCase.GrantAccessToCase(token, userUid, caseUid, privateKey);
                return result;
            };
            return _basicRequestHandler.HandleRequest(lambda, _errorHandler);
        }

        public IActionResult RemoveAccessToCase(Guid userUid, Guid caseUid)
        {
            LambdaDelegate lambda = () =>
            {
                string token = GetToken();
                ResultBase result = _dlCase.RemoveAccessToCase(token, userUid, caseUid);
                return result;
            };
            return _basicRequestHandler.HandleRequest(lambda, _errorHandler);
        }

#endregion

//----------------------------------------------    Запись по делу    -------------------------------------------------------

        #region NOTE

        public IActionResult AddNewNoteToCase(NewNote_In note, IFormFile[] files, Guid caseUid, string privateKey)
        {
            LambdaDelegate lambda = () =>
            {
                string token = GetToken();
                ResultBase result = _dlNote.AddNewNoteToCase(token, note, files, caseUid, privateKey);
                return result;
            };
            return _basicRequestHandler.HandleRequest(lambda, _errorHandler);
        }

        public IActionResult RemoveNoteFromCase(Guid caseUid, Guid noteUid)
        {
            LambdaDelegate lambda = () =>
            {
                string token = GetToken();
                ResultBase result = _dlNote.RemoveNoteFromCase(token, caseUid, noteUid);
                return result;
            };
            return _basicRequestHandler.HandleRequest(lambda, _errorHandler);
        }
        
        #endregion

//----------------------------------------------    Фигуранты по делу    -------------------------------------------------------

        #region CASE_FIGURANTS

        public IActionResult AddNewFigurantToCase(NewCase_In figurant, Guid caseUid, string privateKey)
        {
            LambdaDelegate lambda = () =>
            {
                string token = GetToken();
                ResultBase result = _dlFigurant.AddNewFigurantToCase(token, figurant, caseUid, privateKey);
                return result;
            };
            return _basicRequestHandler.HandleRequest(lambda, _errorHandler);
        }
        public IActionResult RemoveFigurantFromCase(Guid caseUid, Guid figurantUid)
        {
            LambdaDelegate lambda = () =>
            {
                string token = GetToken();
                ResultBase result = _dlFigurant.RemoveFigurantFromCase(token, caseUid, figurantUid);
                return result;
            };
            return _basicRequestHandler.HandleRequest(lambda, _errorHandler);
        }

        #endregion


    }
}