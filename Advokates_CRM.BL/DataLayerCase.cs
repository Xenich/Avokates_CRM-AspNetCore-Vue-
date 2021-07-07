using Advokates_CRM.BL.Helpers;
using Advokates_CRM.DB.Models;
using Advokates_CRM.Layer_Interfaces;
using Advokates_CRM_DTO.Outputs;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Advokates_CRM_DTO.Inputs;

namespace Advokates_CRM.BL
{
    public class DataLayerCase : DataLayerBase, IDataLayerCase
    {
        public DataLayerCase(LawyerCRMContext context) : base(context)
        {
        }

        /// <summary>
        /// Получение информации по делу
        /// </summary>
        /// <param name="token">Токен пользователя</param>
        /// <param name="caseIdPerCompany">Id дела в пересчёте на компанию в БД</param>
        /// <param name="privateKey">Приватный ключ пользователя</param>
        /// <returns></returns>
        public GetCase_Out GetCase(string token, int caseIdPerCompany, string privateKey, int elementsCount = 10, int currentPage = 1)
        {

            GetCase_Out result = new GetCase_Out();

            //string userUidFromToken = HelperSecurity.GetUserUidByJWT(token);
            //Dictionary<string, string> jwtValues = HelperSecurity.GetJWTClaimsValues(token);
            //int companyId = int.Parse(jwtValues["companyId"]);

            JWTClaims JWTValues = HelperSecurity.GetJWTClaimsValues(token);
            Guid userUidFromToken = JWTValues.employeeUid;
            Guid companyUidFromToken = JWTValues.companyUid;
            string userRole = JWTValues.role;



            EmployeeCase employeeCase = (from EmployeeCase ec in _context.EmployeeCase
                                         join c in _context.Case on ec.CaseUid equals c.Uid
                                         where userRole == "director" ? (c.CompanyUid == companyUidFromToken && c.IdPerCompany == caseIdPerCompany && ec.EmployeeUid == userUidFromToken) :
                                                                      (c.CompanyUid == companyUidFromToken && c.IdPerCompany == caseIdPerCompany && ec.EmployeeUid == userUidFromToken && !c.IsClosed)
                                         select ec)
                                         .FirstOrDefault();
            if (employeeCase == null)
                return ErrorHandler<GetCase_Out>.SetDBProblem(result, "Нет права доступа. Обратитесь к администратору");

            byte[] symmetricKey = HelperSecurity.DecryptByRSA(privateKey, employeeCase.EncriptedAesKey);

            var _case = (from c in _context.Case
                         join cm in _context.Company on c.CompanyUid equals cm.Uid
                         where cm.Uid == companyUidFromToken && c.IdPerCompany == caseIdPerCompany
                         select new
                         {
                             Title = c.Title,
                             Info = HelperSecurity.DecriptByAes(c.Info, symmetricKey),
                             DateCreate = c.Date.Value.ToShortDateString(),
                             UpdateDate = c.UpdateDate.Value.ToShortDateString() + " " + c.UpdateDate.Value.ToShortTimeString(),
                             IsClosed = c.IsClosed,
                             UID = c.Uid
                         }).FirstOrDefault();

            result.CanManage = userRole == "director" || employeeCase.IsOwner;
            result.Title = _case.Title;
            result.Info = _case.Info;
            result.DateCreate = _case.DateCreate;
            result.UpdateDate = _case.UpdateDate;
            result.IsClosed = _case.IsClosed;
            result.CaseUid = _case.UID;

            result.EmployeesWithAccess = (from e in _context.Employee
                                          join ec in _context.EmployeeCase on e.Uid equals ec.EmployeeUid
                                          join rLeft in _context.Role on e.RoleUid equals rLeft.Uid into rTemp
                                          from r in rTemp.DefaultIfEmpty()
                                          where ec.CaseUid == _case.UID
                                          select new Case_Employee()
                                          {
                                              Name = (string.IsNullOrEmpty(e.Surname) ? "" : e.Surname) + " " + (string.IsNullOrEmpty(e.Name) ? "" : e.Name) + " " + (string.IsNullOrEmpty(e.SecondName) ? "" : e.SecondName),
                                              EmployeeUid = e.Uid,
                                              IsOwner = ec.IsOwner,
                                              CanManageThisEmployee = (userRole == "director" && r.RoleName != "director" && !ec.IsOwner) ||
                                                                      (employeeCase.IsOwner && r.RoleName != "director" && userUidFromToken != e.Uid)
                                              //IsDirector = r.RoleName=="director"
                                          }).ToArray();

            result.EmployeesWithoutAccess = (from e in _context.Employee
                                             where e.CompanyUid == companyUidFromToken && !result.EmployeesWithAccess
                                                                                                     .Select(ee => ee.EmployeeUid)
                                                                                                     .Contains(e.Uid)
                                             select new Case_Employee()
                                             {
                                                 Name = (string.IsNullOrEmpty(e.Surname) ? "" : e.Surname) + " " + (string.IsNullOrEmpty(e.Name) ? "" : e.Name) + " " + (string.IsNullOrEmpty(e.SecondName) ? "" : e.SecondName),
                                                 EmployeeUid = e.Uid,
                                                 IsOwner = false,
                                                 CanManageThisEmployee = (userRole == "director" || employeeCase.IsOwner)
                                                 //IsDirector = false
                                             }).ToArray();

            result.Figurants = (from f in _context.Figurant
                                join rLeft in _context.FigurantRole on f.FigurantRoleUid equals rLeft.Uid into rTemp
                                from r in rTemp.DefaultIfEmpty()
                                where f.CaseUid == result.CaseUid
                                select new Case_Figurant()
                                {
                                    // TODO : доделать проверку на NULL
                                    FullName = (string.IsNullOrEmpty(f.Surname) ? "" : f.Surname) + " " + (string.IsNullOrEmpty(f.Name) ? "" : f.Name) + " " + (string.IsNullOrEmpty(f.SecondName) ? "" : f.SecondName),
                                    Uid = f.Uid,
                                    Phone = (string.IsNullOrEmpty(f.Phone) ? "" : f.Phone),
                                    Role = r.RoleName
                                }).ToArray();

            result.Notes = DBHelper.GetCaseNotes(_case.UID, symmetricKey, userRole, employeeCase.IsOwner, userUidFromToken, elementsCount, currentPage,  _context);

            result.FigurantRoleOptions = DBHelper.GetFigurantRoleOptions(companyUidFromToken, _context);
            result.Status = ResultBase.StatusOk;

            //TODO : доделать метод

            return result;
        }

        /// <summary>
        /// Создание нового дела
        /// </summary>
        /// <param name="inputValue"></param>
        /// <returns></returns>
        public ResultBase CreateNewCase(NewCase_In inputValue)
        {
            ResultBase result = new ResultBase();

            if (string.IsNullOrEmpty(inputValue.Title))
                return ErrorHandler<ResultBase>.SetDBProblem(result, "Дело не создано. Введите название.");
            JWTClaims jWTClaims = HelperSecurity.GetJWTClaimsValues(inputValue.Token);
            Guid userUID = jWTClaims.employeeUid;
            bool isDirector = jWTClaims.role == "director";
            var userInfo = _context.Employee
                                    .Where(e => e.Uid == userUID && e.IsActive.Value)
                                    .Select(e => new
                                    {
                                        publicKey = e.PublicKey,
                                        companyUid = e.CompanyUid
                                    })
                                    .FirstOrDefault();

            int caseId = 0;
            if (_context.Case.Where(c => c.CompanyUid == userInfo.companyUid).Any())
            {
                caseId = _context.Case
                                .Where(c => c.CompanyUid == userInfo.companyUid)
                                .Select(c => c.IdPerCompany)
                                .Max();
            }
            caseId++;

            if (userInfo == null)
                return ErrorHandler<ResultBase>.SetDBProblem(result, "Пользователь не найден");

            Tuple<byte[], byte[]> AESKey = HelperSecurity.CreateAESKeyEncryptedByRSA(userInfo.publicKey);
            byte[] encriptedAesKeyByRSA = AESKey.Item1;
            byte[] aesKey = AESKey.Item2;

            DateTime dateTime = DateTime.Now;
            Case newCase = new Case()
            {
                CompanyUid = userInfo.companyUid,
                Date = dateTime,
                UpdateDate = dateTime,
                Title = inputValue.Title,
                IsClosed = false,
                Info = HelperSecurity.EncryptByAes(string.IsNullOrEmpty(inputValue.Info) ? "" : inputValue.Info, aesKey),
                IdPerCompany = caseId
            };
            _context.Case.Add(newCase);
            _context.SaveChanges();

            EmployeeCase employeeCase = new EmployeeCase()
            {
                CaseUid = newCase.Uid,
                EmployeeUid = userUID,
                IsOwner = true,
                EncriptedAesKey = encriptedAesKeyByRSA
            };
            _context.EmployeeCase.Add(employeeCase);

            if (!isDirector)
            {

                var directorInfo = (from e in _context.Employee
                                    join r in _context.Role on e.RoleUid equals r.Uid
                                    where e.CompanyUid == userInfo.companyUid && r.RoleName == "director"
                                    select new
                                    {
                                        directorUid = e.Uid,
                                        publicKey = e.PublicKey
                                    }).FirstOrDefault();

                encriptedAesKeyByRSA = HelperSecurity.EncryptByRSA(directorInfo.publicKey, aesKey);
                EmployeeCase directorEmployeeCase = new EmployeeCase()
                {
                    CaseUid = newCase.Uid,
                    EmployeeUid = directorInfo.directorUid,
                    IsOwner = false,
                    EncriptedAesKey = encriptedAesKeyByRSA
                };
                _context.EmployeeCase.Add(directorEmployeeCase);
            }

            if (inputValue.Figurants != null)
            {
                foreach (NewCase_Figurant_In figurant in inputValue.Figurants)
                {
                    Figurant newFigurant = new Figurant()
                    {
                        CaseUid = newCase.Uid,
                        Email = figurant.Email,
                        FigurantRoleUid = figurant.RoleUid,
                        Name = figurant.Name,
                        Surname = figurant.Surname,
                        SecondName = figurant.SecondName,
                        Phone = figurant.Phone,
                        Description = HelperSecurity.EncryptByAes(string.IsNullOrEmpty(figurant.Description) ? "" : figurant.Description, aesKey)
                    };
                    _context.Figurant.Add(newFigurant);
                }
            }
            _context.SaveChanges();

            result.Status = ResultBase.StatusOk;
            // TODO : доделать метод

            return result;
        }

        public GetCaseNotes_Out GetCaseNotes(string token, Guid caseUid, string privateKey, int elementsCount, int currentPage)
        {
            GetCaseNotes_Out result = new GetCaseNotes_Out();

            JWTClaims JWTValues = HelperSecurity.GetJWTClaimsValues(token);
            Guid userUidFromToken = JWTValues.employeeUid;
            Guid companyUidFromToken = JWTValues.companyUid;
            string userRole = JWTValues.role;

            EmployeeCase employeeCase = (from EmployeeCase ec in _context.EmployeeCase
                                         join c in _context.Case on ec.CaseUid equals c.Uid
                                         where userRole == "director" ? (c.CompanyUid == companyUidFromToken && c.Uid == caseUid && ec.EmployeeUid == userUidFromToken) :
                                                                      (c.CompanyUid == companyUidFromToken && c.Uid == caseUid && ec.EmployeeUid == userUidFromToken && !c.IsClosed)
                                         select ec)
                         .FirstOrDefault();
            if (employeeCase == null)
                return ErrorHandler<GetCaseNotes_Out>.SetDBProblem(result, "Нет права доступа. Обратитесь к администратору");

            byte[] symmetricKey = HelperSecurity.DecryptByRSA(privateKey, employeeCase.EncriptedAesKey);
            result.Notes = DBHelper.GetCaseNotes(caseUid, symmetricKey, userRole, employeeCase.IsOwner, userUidFromToken, elementsCount, currentPage, _context);

            result.Status = ResultBase.StatusOk;
            return result;
        }

        /// <summary>
        /// Открывает доступ к делу определенному сотруднику
        /// </summary>
        /// <param name="token"></param>
        /// <param name="userUid">Сотрудник, которому надо открыть доступ</param>
        /// <param name="caseUid">Дело, к которому надо открыть доступ</param>
        /// <param name="privateKey">Приватный ключ открывающего (для расшифровки симметричного ключа)</param>
        /// <returns></returns>
        public ResultBase GrantAccessToCase(string token, Guid userUid, Guid caseUid, string privateKey)
        {
            ResultBase result = new ResultBase();

            JWTClaims JWTValues = HelperSecurity.GetJWTClaimsValues(token);
            Guid userUidFromToken = JWTValues.employeeUid;
            Guid companyUidFromToken = JWTValues.companyUid;
            string userRole = JWTValues.role;

            if (userRole != "director")
            {
                Guid ownerUID = _context.EmployeeCase
                                        .Where(e => e.CaseUid == caseUid && e.IsOwner)
                                        .Select(e => e.EmployeeUid)
                                        .FirstOrDefault();

                if (userUidFromToken != ownerUID)
                {
                    return ErrorHandler<ResultBase>.SetDBProblem(result, "Недостаточно прав для осуществления данной операции");
                }
            }

            // проверяем, есть ли у юзера уже доступ
            EmployeeCase employeeCase = _context.EmployeeCase.Where(e => e.EmployeeUid == userUid && e.CaseUid == caseUid).FirstOrDefault();
            if (employeeCase != null)
                return ErrorHandler<ResultBase>.SetDBProblem(result, "У пользователя уже есть доступ к данному делу");

            // Если нет:
            byte[] employeePublicKey = _context.Employee
                                                .Where(e => e.Uid == userUid)
                                                .Select(e => e.PublicKey)
                                                .FirstOrDefault();

            byte[] encriptedAesKey = _context.EmployeeCase
                                        .Where(e => e.CaseUid == caseUid && e.EmployeeUid == userUidFromToken)
                                        .Select(e => e.EncriptedAesKey)
                                        .FirstOrDefault();
            // если у этого юзера нет доступа к делу (это сотрудник другой компании), то encriptedAesKey = null
            if (encriptedAesKey == null)
                return ErrorHandler<ResultBase>.SetDBProblem(result, "Нет доступа к делу");

            byte[] aesKey = HelperSecurity.DecryptByRSA(privateKey, encriptedAesKey);

            employeeCase = new EmployeeCase()
            {
                CaseUid = caseUid,
                EmployeeUid = userUid,
                IsOwner = false,
                EncriptedAesKey = HelperSecurity.EncryptByRSA(employeePublicKey, aesKey)
            };
            _context.EmployeeCase.Add(employeeCase);
            _context.SaveChanges();

            result.Status = ResultBase.StatusOk;
            return result;
        }

        /// <summary>
        /// Закрывает доступ сотрудника к делу
        /// </summary>
        /// <param name="token"></param>
        /// <param name="employeeUid">Сотрудник, которому надо закрыть доступ</param>
        /// <param name="caseUid">Дело, к которому надо закрыть дступ</param>
        /// <returns></returns>
        public ResultBase RemoveAccessToCase(string token, Guid employeeUid, Guid caseUid)
        {
            ResultBase result = new ResultBase();

            JWTClaims JWTValues = HelperSecurity.GetJWTClaimsValues(token);
            Guid userUidFromToken = JWTValues.employeeUid;
            Guid companyUidFromToken = JWTValues.companyUid;
            string userRole = JWTValues.role;

            Guid ownerUID = _context.EmployeeCase
                                    .Where(e => e.CaseUid == caseUid && e.IsOwner)
                                    .Select(e => e.EmployeeUid)
                                    .FirstOrDefault();

            if (!(userRole == "director" || userUidFromToken == ownerUID))
                return ErrorHandler<ResultBase>.SetDBProblem(result, "Недостаточно прав для осуществления данной операции");

            var employeeInfo = (from e in _context.Employee
                                join r in _context.Role on e.RoleUid equals r.Uid
                                join ec in _context.EmployeeCase on e.Uid equals ec.EmployeeUid
                                where e.Uid == employeeUid && ec.CaseUid == caseUid && e.CompanyUid == companyUidFromToken
                                select new
                                {
                                    isOwner = ec.IsOwner,
                                    role = r.RoleName
                                }).FirstOrDefault();

            if (employeeInfo == null)
            {
                result.Status = ResultBase.StatusOk;
                return result;
            }

            if (employeeInfo.isOwner || employeeInfo.role == "director")
            {
                return ErrorHandler<ResultBase>.SetDBProblem(result, "Невозможно закрыть доступ к делу данному пользователю");
            }

            EmployeeCase employeeCase = _context.EmployeeCase.FirstOrDefault(e => e.CaseUid == caseUid && e.EmployeeUid == employeeUid);
            if (employeeCase != null)
            {
                _context.EmployeeCase.Remove(employeeCase);
                _context.SaveChanges();
            }

            result.Status = ResultBase.StatusOk;
            return result;
        }

        public NewCaseGetModel_Out NewCaseGetModel(string token)
        {
            NewCaseGetModel_Out result = new NewCaseGetModel_Out();
            Guid companyUid = HelperSecurity.GetCompanyUidByJWT(token);
            result.FigurantRoleOptions = DBHelper.GetFigurantRoleOptions(companyUid, _context);

            return result;
        }
    }
}
