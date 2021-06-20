using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Advokates_CRM_DTO.Inputs;
using Advokates_CRM_DTO.Outputs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Http;
using Advokates_CRM.Layer_Interfaces;
using Advokates_CRM.BL.Helpers;

namespace Avokates_CRM.Controllers
{
    [Authorize]
    public class DataController : BaseController
    {
        private readonly IDataLayer _dl;      // dataLayer
        private readonly IDataLayerCabinet _dlCabinet;
        private readonly IDataLayerCase _dlCase;
        private readonly IDataLayerNote _dlNote;
        private readonly IErrorHandler _errorHandler;
        public DataController(IDataLayer dataLayer, IDataLayerCabinet dlCabinet, IDataLayerCase dlCase, IDataLayerNote dlNote, IErrorHandler errorHandler)      // в Startup : AddScoped<IDataLayer, DataLayer>();
        {
            _dl = dataLayer;
            _dlCabinet = dlCabinet;
            _dlCase = dlCase;
            _dlNote = dlNote;
            _errorHandler = errorHandler;

        }

        public IActionResult Test([FromBody] BaseAuth_In auth)
        {
            var c = HttpContext.Request;
            if (HelperSecurity.IsTokenValid(auth.Token))
            {
                GetCasesList_Out result = _dl.GetCasesList(auth.Token);
                return Ok(result);
            }
            else
                return Ok(ErrorHandler<ResultBase>.TokenNotValid());

        }

        [Authorize(Roles = "admin, director")]
        public IActionResult CreateInvite(string email)
        {          
            ResultBase result = _dl.CreateInvite(GetToken(), email);
            return Ok(result);
        }

        // Создание нового юзера по пригласительному токену
        [AllowAnonymous]
        public IActionResult CreateUserByInvite(Registration_In value)
        {
            Registration_Out result = _dl.CreateUserByInvite(value);
            if (result.Status == ResultBase.StatusOk)
            {
                HttpContext.Session.SetString("token", result.JWT);
            }
            return Ok(result);
        }


        public IActionResult GetMainPage()
        {
            string token = GetToken();
            GetMainPage_Out result = _dl.GetMainPage(token);
            return Ok(result);
        }

        public IActionResult GetCasesList()
        {            
            string token = GetToken();
            if (HelperSecurity.IsTokenValid(token))
            {
                GetCasesList_Out result = _dl.GetCasesList(token);
                return Ok(result);
            }
            else
                return Ok(ErrorHandler<ResultBase>.TokenNotValid());
        }

        #region CASE_FIGURANTS

        public IActionResult AddNewFigurantToCase(NewCase_In figurant, Guid caseUid, string privateKey)
        {
            string token = GetToken();
            ResultBase result = _dl.AddNewFigurantToCase(token, figurant, caseUid, privateKey);
            return Ok(result);
        }
        public IActionResult RemoveFigurantFromCase( Guid caseUid, Guid figurantUid)
        {
            string token = GetToken();
            ResultBase result = _dl.RemoveFigurantFromCase(token, caseUid, figurantUid);
            return Ok(result);
        }

        #endregion

        //----------------------------------------------    Запись по делу    -------------------------------------------------------

        #region NOTE

        public IActionResult AddNewNoteToCase(NewNote_In note, IFormFile[] files, Guid caseUid, string privateKey)
        {
            Lambda lambda = () =>
            {
                string token = GetToken();
                ResultBase result = _dlNote.AddNewNoteToCase(token, note, files, caseUid, privateKey);
                return result;
            };
            return HandleRequestBasic(lambda);
        }

        public IActionResult RemoveNoteFromCase(Guid caseUid, Guid noteUid)
        {
            Lambda lambda = () =>
            {
                string token = GetToken();
                ResultBase result = _dlNote.RemoveNoteFromCase(token, caseUid, noteUid);
                return result;
            };
            return HandleRequestBasic(lambda);
        }
        
        #endregion

//----------------------------------------------    ДЕЛО    -------------------------------------------------------

        #region CASE

        public IActionResult CreateNewCase(NewCase_In value)
        {
            Lambda lambda = () =>
            {
                value.Token = GetToken();
                ResultBase result = _dlCase.CreateNewCase(value);
                return result;
            };
            return HandleRequestBasic(lambda);
        }

        public IActionResult NewCaseGetModel()
        {
            Lambda lambda = () =>
            {
                string token = GetToken();
                NewCaseGetModel_Out result = _dlCase.NewCaseGetModel(token);
                return result;
            };
            return HandleRequestBasic(lambda);
        }

            // метод вызывается при обновлении списка записей (например при добавлении новой записи)
        public IActionResult GetCaseNotes(Guid caseUid, string privateKey)
        {
            Lambda lambda = () =>
            {
                string token = GetToken();
                GetCaseNotes_Out result = _dlCase.GetCaseNotes(token, caseUid, privateKey);
                return result;
            };
            return HandleRequestBasic(lambda);
        }

        public IActionResult GetCaseInfo(int id, string privateKey)
        {
            Lambda lambda = () =>
            {
                string token = GetToken();
                GetCase_Out result = _dlCase.GetCase(token, id, privateKey);
                return result;
            };
            return HandleRequestBasic(lambda);
        }

        public IActionResult GrantAccessToCase(Guid userUid, Guid caseUid, string privateKey)
        {
            Lambda lambda = () =>
            {
                string token = GetToken();
                ResultBase result = _dlCase.GrantAccessToCase(token, userUid, caseUid, privateKey);
                return result;
            };
            return HandleRequestBasic(lambda);
        }

        public IActionResult RemoveAccessToCase(Guid userUid, Guid caseUid)
        {
            Lambda lambda = () =>
            {
                string token = GetToken();
                ResultBase result = _dlCase.RemoveAccessToCase(token, userUid, caseUid);
                return result;
            };
            return HandleRequestBasic(lambda);
        }

#endregion

//----------------------------------------------    Личный кабинет      -----------------------------------------------------------------------------------

        #region CABINET

        delegate ResultBase Lambda();

        public IActionResult GetCabinetInfo()
        {
            Lambda lambda = ()=>
            {
                string token = GetToken();
                GetCabinetInfo_Out result = _dlCabinet.GetCabinetInfo(token);
                return result;
            };
            return HandleRequestBasic(lambda);
        }

        public IActionResult CabinetInfoSaveChanges(CabinetInfo_In cabinetInfo)
        {
            Lambda lambda = () =>
            {
                string token = GetToken();
                ResultBase result = _dlCabinet.CabinetInfoSaveChanges(token, cabinetInfo);
                return result;
            };
            return HandleRequestBasic(lambda);
        }

        #endregion

        private IActionResult HandleRequestBasic(Lambda lambda)
        {
            ResultBase result;
            try
            {
                result = lambda();
                if(string.IsNullOrEmpty(result.Status))
                    result.Status = ResultBase.StatusOk;
                return Ok(result);
            }
            catch (Exception ex)
            {
                result = new ResultBase();
                result.Status = ResultBase.StatusBad;
                return Ok(_errorHandler.SetDBProblem<ResultBase>(result, ex.Message));
            }
        }
    }
}