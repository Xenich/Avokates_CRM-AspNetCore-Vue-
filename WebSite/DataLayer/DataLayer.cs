using Avokates_CRM.Models.Outputs;
using Avokates_CRM;

using Avokates_CRM.Models.DB;
using Avokates_CRM.Models.Inputs;
using StoredProcedureEFCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using WebSite.Models.Outputs;
using WebSite.Helpers;

namespace WebSite.DataLayer
{
    public class DataLayerDB : IDataLayer
    {
        private readonly LawyerCRMContext _context;
        public DataLayerDB(LawyerCRMContext context)
        {
            _context = context;
        }

        public string CheckConnectionToDB()
        {
            try
            {
                _context.LoadStoredProc("CheckConnection")
                           .AddParam("value", 100)
                           .AddParam("return", out IOutParam<int> Outret)
                           .ExecNonQuery();
                if (Outret.Value == 101)
                    return "Связь установлена";
                else
                    return "Связь с БД отсутствует";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public GetMainPage_Out GetMainPage(string token)
        {
            GetMainPage_Out result = new GetMainPage_Out();

            try
            {
                int companyIdFromToken = HelperSecurity.GetCompanyIdByJWT(token);
                int userIdFromToken = HelperSecurity.GetUserIdByJWT(token);

                var r = (from c in _context.Company
                         join e in _context.Employee on c.Uid equals e.CompanyUid
                         where e.Id == userIdFromToken
                         select new
                         {
                             userName = ((string.IsNullOrEmpty(e.Surname)?"": e.Surname) + " " + (string.IsNullOrEmpty(e.Name) ? "" : e.Name) + " "
                             + (string.IsNullOrEmpty(e.SecondName) ? "" : e.SecondName)),
                             companyName = c.CompanyName
                         }).FirstOrDefault();

                result.Name = r.userName;
                result.CompanyName = r.companyName;


                result.CasesCount = (from e in _context.Employee
                                     join ec in _context.EmployeeCase on e.Uid equals ec.EmployeeUid
                                     where e.Id == userIdFromToken
                                     select ec).Count();

                result.NotesCount = (from e in _context.Employee
                                     join n in _context.Note on e.Uid equals n.EmployeeUid
                                     where e.Id == userIdFromToken
                                     select n).Count();

                result.Status = ResultBase.StatusOk;
            }
            catch (Exception ex)
            {
                ErrorHandler<GetMainPage_Out>.SetDBProblem(result, ex.Message);
            }
            return result;
        }

        public GetCasesList_Out GetCasesList(string token)
        {
            GetCasesList_Out result = new GetCasesList_Out();
            try
            {
                Guid userUidFromToken = HelperSecurity.GetUserUidByJWT(token);
                if (userUidFromToken == Guid.Empty)
                {
                    return ErrorHandler<GetCasesList_Out>.SetDBProblem(result,"Неверный идентификатор пользователя");
                }
                result.CasesList = (
                    from ec in _context.EmployeeCase
                    join c in _context.Case on ec.CaseUid equals c.Uid
                    where ec.EmployeeUid == userUidFromToken && !c.IsClosed
                    select new Case_Out()
                    {
                        //Uid = c.Uid,
                        CaseTitle = c.Title,
                        CreateDate = c.Date.HasValue? c.Date.Value.ToShortDateString():"",
                        IdPerCompany = c.IdPerCompany,
                        UpdateDate = c.UpdateDate.HasValue? c.UpdateDate.Value.ToShortDateString():"",
                        CaseOwner = (
                              from ee in _context.Employee
                              join ecc in _context.EmployeeCase on ee.Uid equals ecc.EmployeeUid
                              where ecc.CaseUid == c.Uid && ecc.IsOwner
                              select ((string.IsNullOrEmpty(ee.Surname) ? "" : ee.Surname) + " " + (string.IsNullOrEmpty(ee.Name) ? "" : ee.Name) + " "
                              + (string.IsNullOrEmpty(ee.SecondName) ? "" : ee.SecondName))
                              ).FirstOrDefault()
                    })
                    .OrderBy(b=>b.UpdateDate)
                    .ToList();
                result.Status = ResultBase.StatusOk;
            }
            catch (Exception ex)
            {
                ErrorHandler<GetCasesList_Out>.SetDBProblem(result, ex.Message);
            }
            return result;           
        }

        public GetCase_Out GetCase(string token, int caseId)
        {

            GetCase_Out result = new GetCase_Out();
            try
            {
                //string userUidFromToken = HelperSecurity.GetUserUidByJWT(token);
                Dictionary<string, string> jwtValues =   HelperSecurity.GetJWTClaimsValues(token);
                int companyId = int.Parse(jwtValues["companyId"]);



                // var r = from 
                Case _case = (from c in _context.Case
                              join cm in _context.Company on c.CompanyUid equals cm.Uid
                              where cm.Id == companyId && c.IdPerCompany == caseId && !c.IsClosed
                              select c).FirstOrDefault();

                result.Title = _case.Title;
            }
            catch (Exception ex)
            {
                ErrorHandler<GetCase_Out>.SetDBProblem(result, ex.Message);
            }
            return result;
        }

        public GetCabinetInfo_Out GetCabinetInfo(string token)
        {
            GetCabinetInfo_Out result;
            try
            {
                Guid userUidFromToken = HelperSecurity.GetUserUidByJWT(token);
                //Employee emp = _context.Employee.FirstOrDefault(e => e.Uid == userUidFromToken);
                result = (from e in _context.Employee
                          join rLeft in _context.Role on e.RoleUid equals rLeft.Uid into rTemp
                          from r in rTemp.DefaultIfEmpty()
                          where e.Uid == userUidFromToken
                          select new GetCabinetInfo_Out
                          {
                              Name = e.Name,
                              Surname = e.Surname,
                              SecondName = e.SecondName,
                              PublicKey = e.PublicKey==null?"": Convert.ToBase64String(e.PublicKey),
                              Birthday = e.Birthday.Value.Date.ToString(),
                              Email = e.Email,
                              Phone = e.Phone,
                              Role = r.RoleName
                          }).FirstOrDefault();
                result.Status = ResultBase.StatusOk;

            }
            catch (Exception ex)
            {
                result = new GetCabinetInfo_Out();
                ErrorHandler<GetCabinetInfo_Out>.SetDBProblem(result, ex.Message);
            }
            return result;
        }

        public ResultBase CabinetInfoSaveChanges(string token, GetCabinetInfo_Out cabinetInfo)
        {
            ResultBase result = new ResultBase();
            try
            {
                Guid userUidFromToken = HelperSecurity.GetUserUidByJWT(token);
                Employee emp = _context.Employee.FirstOrDefault(e => e.Uid == userUidFromToken);
                emp.Surname = cabinetInfo.Surname;
                emp.Name = cabinetInfo.Name;
                emp.SecondName = cabinetInfo.SecondName;
                emp.Phone = cabinetInfo.Phone;
                emp.Email = cabinetInfo.Email;

                DateTime bDay;
                bool bDaySuccessParse =  DateTime.TryParse(cabinetInfo.Birthday, out bDay);
                if (bDaySuccessParse)
                    emp.Birthday = bDay;

                _context.SaveChanges();
                result.Status = ResultBase.StatusBad;
            }
            catch (Exception ex)
            {
                ErrorHandler<ResultBase>.SetDBProblem(result, ex.Message);
            }
            return result;
        }

        //public ResultBase GetFigurantRoles(BaseAuth_In inputValue)
        //{
        //    FigurantRoles_Out result = new FigurantRoles_Out();
        //    bool tokenValid = HelperSecurity.IsTokenValid(inputValue.Token);
        //    try
        //    {
        //        if (tokenValid)
        //        {
        //            result.figurantRolesDictionary = HelperDB.figurantRolesDictionary;
        //            result.Status = ResultBase.StatusOk;
        //            return result;
        //        }
        //        else
        //        {
        //            result.ErrorMessages = new List<ErrorMessageResult>() { new ErrorMessageResult() { message = "Tокен недействителен или просрочен" } };
        //            result.Status = ResultBase.BadToken;
        //            return result;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return HelperDB.DBProblem(ex.Message);
        //    }
        //}


        //public ResultBase CreateNewCase(NewCase_In inputValue)
        //{
        //    ResultBase result = new ResultBase();
        //    bool tokenValid = HelperSecurity.IsTokenValid(inputValue.Token);
        //    try
        //    {
        //        if (tokenValid)
        //        {
        //            Dictionary<string, string> JWTClaimsValues = HelperSecurity.GetJWTClaimsValues(inputValue.Token);

        //            _context.LoadStoredProc("prCreateNewCase")
        //                .AddParam("title", inputValue.Title)
        //                .AddParam("info", inputValue.Info)
        //                .AddParam("figurants", Newtonsoft.Json.JsonConvert.SerializeObject(inputValue.Figurants))
        //                .AddParam("employeeId", JWTClaimsValues["employeeId"])
        //                .AddParam("companyUID", JWTClaimsValues["companyUID"])
        //                .ReturnValue(out IOutParam<int> retparam)
        //                .ExecNonQuery();

        //            switch (retparam.Value)
        //            {
        //                case 6:
        //                    {
        //                        result.ErrorMessages = new List<ErrorMessageResult>() { new ErrorMessageResult() { message = "Id пользователя не соответствует UID компании." } };
        //                        result.Status = ResultBase.StatusBad;
        //                        return result;
        //                    }
        //            }
        //            result.Status = ResultBase.StatusOk;
        //            return result;
        //        }
        //        else
        //        {
        //            result.ErrorMessages = new List<ErrorMessageResult>() { new ErrorMessageResult() { message = "Tокен недействителен или просрочен" } };
        //            result.Status = ResultBase.BadToken;
        //            return result;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return HelperDB<>.DBProblem(ex.Message);
        //    }


        //}
    }


}
