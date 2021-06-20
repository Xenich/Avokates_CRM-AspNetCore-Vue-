using Advokates_CRM_DTO.Outputs;
using Advokates_CRM_DTO.Inputs;
using StoredProcedureEFCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using System.IO;
using Advokates_CRM.Layer_Interfaces;
using Advokates_CRM.DB.Models;
using Advokates_CRM.BL.Helpers;
using Advokates_CRM.BL.Objects;

namespace Advokates_CRM.BL
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

// --------------------------------------------------    НОВЫЙ ПОЛЬЗОВАТЕЛЬ СИСТЕМЫ      ---------------------------------------------------
        #region Новый пользователь системы



        #endregion
        // -----------------------------------------------------------------------------------------------------------------------------------------

        public GetMainPage_Out GetMainPage(string token)
        {
            GetMainPage_Out result = new GetMainPage_Out();
            //int companyIdFromToken = HelperSecurity.GetCompanyIdByJWT(token);
            //int userIdFromToken = HelperSecurity.GetUserIdByJWT(token);
            Guid userUid = HelperSecurity.GetUserUidByJWT(token);

            var r = (from c in _context.Company
                     join e in _context.Employee on c.Uid equals e.CompanyUid
                     where e.Uid == userUid
                     select new
                     {
                         userName = ((string.IsNullOrEmpty(e.Surname) ? "" : e.Surname) + " " + (string.IsNullOrEmpty(e.Name) ? "" : e.Name) + " "
                         + (string.IsNullOrEmpty(e.SecondName) ? "" : e.SecondName)),
                         companyName = c.CompanyName
                     }).FirstOrDefault();

            result.Name = r.userName;
            result.CompanyName = r.companyName;


            result.CasesCount = (from ec in _context.EmployeeCase
                                 where ec.EmployeeUid == userUid
                                 select ec).Count();

            result.NotesCount = (from e in _context.Employee
                                 join n in _context.Note on e.Uid equals n.EmployeeUid
                                 where e.Uid == userUid
                                 select n).Count();
            result.isAdmin = HelperSecurity.IsTokenAdmin(token);
            result.Status = ResultBase.StatusOk;

            return result;
        }

        // --------------------------------------------------    ДЕЛО      -------------------------------------------------------------------------
        #region ДЕЛО

        public GetCasesList_Out GetCasesList(string token)
        {
            GetCasesList_Out result = new GetCasesList_Out();

            JWTClaims JWTValues = HelperSecurity.GetJWTClaimsValues(token);
            Guid userUidFromToken = JWTValues.employeeUid;
            Guid companyUidFromToken = JWTValues.companyUid;
            string userRole = JWTValues.role;

            if (userUidFromToken == Guid.Empty)
            {
                return ErrorHandler<GetCasesList_Out>.SetDBProblem(result, "Неверный идентификатор пользователя");
            }
            if (userRole == "director")
            {
                result.CasesList = (
                from c in _context.Case
                where !c.IsClosed && c.CompanyUid == companyUidFromToken
                select new Case_Out()
                {
                        //Uid = c.Uid,
                        CaseTitle = c.Title,
                    CreateDate = c.Date.HasValue ? c.Date.Value.ToShortDateString() : "",
                    IdPerCompany = c.IdPerCompany,
                    UpdateDate = c.UpdateDate.HasValue ? c.UpdateDate.Value.ToShortDateString() : "",
                    CaseOwner = (
                          from ee in _context.Employee
                          join ecc in _context.EmployeeCase on ee.Uid equals ecc.EmployeeUid
                          where ecc.CaseUid == c.Uid && ecc.IsOwner
                          select ((string.IsNullOrEmpty(ee.Surname) ? "" : ee.Surname) + " " + (string.IsNullOrEmpty(ee.Name) ? "" : ee.Name) + " "
                          + (string.IsNullOrEmpty(ee.SecondName) ? "" : ee.SecondName))
                          ).FirstOrDefault(),
                    Uid = c.Uid
                })
                .OrderBy(b => b.UpdateDate)
                .ToList();
            }
            else
            {
                result.CasesList = (
                    from ec in _context.EmployeeCase
                    join c in _context.Case on ec.CaseUid equals c.Uid
                    where ec.EmployeeUid == userUidFromToken && !c.IsClosed && c.CompanyUid == companyUidFromToken
                    select new Case_Out()
                    {
                            //Uid = c.Uid,
                            CaseTitle = c.Title,
                        CreateDate = c.Date.HasValue ? c.Date.Value.ToShortDateString() : "",
                        IdPerCompany = c.IdPerCompany,
                        UpdateDate = c.UpdateDate.HasValue ? c.UpdateDate.Value.ToShortDateString() : "",
                        CaseOwner = (
                              from ee in _context.Employee
                              join ecc in _context.EmployeeCase on ee.Uid equals ecc.EmployeeUid
                              where ecc.CaseUid == c.Uid && ecc.IsOwner
                              select ((string.IsNullOrEmpty(ee.Surname) ? "" : ee.Surname) + " " + (string.IsNullOrEmpty(ee.Name) ? "" : ee.Name) + " "
                              + (string.IsNullOrEmpty(ee.SecondName) ? "" : ee.SecondName))
                              ).FirstOrDefault(),
                        Uid = c.Uid
                    })
                    .OrderBy(b => b.UpdateDate)
                    .ToList();
            }
            result.Status = ResultBase.StatusOk;

            return result;
        }



        #endregion
// -----------------------------------------------------------------------------------------------------------------------------------------
    }
}
