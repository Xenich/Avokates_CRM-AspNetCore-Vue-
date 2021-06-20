using Advokates_CRM_DTO.Outputs;
using Advokates_CRM_DTO.Inputs;
using StoredProcedureEFCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MimeKit;
using MailKit.Net.Smtp;
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

        // создание приглашения новому пользователю путём отсылки ему email с токеном приглашения
        public ResultBase CreateInvite(string token, string email)
        {
            ResultBase result = new ResultBase();
            try
            {
                Guid companyUid = HelperSecurity.GetCompanyUidByJWT(token);
                Guid userUID = HelperSecurity.GetUserUidByJWT(token);

                email = email.Trim();
                email = email.ToLower();
                string pattern = @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                                 @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$";
                if (Regex.Matches(email, pattern, RegexOptions.IgnoreCase).Count != 1)
                {
                    ErrorHandler<ResultBase>.SetDBProblem(result, "Email not valid");
                    return result;
                }
                SHA256 sha256 = SHA256.Create();
                byte[] bytes = Encoding.UTF8.GetBytes(email);
                string inviteToken = Convert.ToBase64String(sha256.ComputeHash(bytes));

                Invite invite = _context.Invite.FirstOrDefault(i => i.Token == inviteToken);
                if (invite == null)
                {
                    invite = new Invite()
                    {
                        CompanyUid = companyUid,
                        Token = inviteToken
                    };
                    _context.Invite.Add(invite);
                }
                invite.ExpiresIn = DateTime.Now.AddHours(1);
                string message = "Для регистрации перейдите по ссылке ниже" + Environment.NewLine;
                message += BaseHelper.GetHostAddress() + "Home/Invite?token=" + inviteToken;
                SendEmail(email, "Регистрация Advokates CRM", message);
                _context.SaveChanges();
                result.Status = ResultBase.StatusOk;
            }
            catch (Exception ex)
            {
                ErrorHandler<ResultBase>.SetDBProblem(result, ex.Message);
            }
            return result;
        }

        public async static Task SendEmail(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Администрация сайта", "blondinkaTest1@gmail.com"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 25, false);
                await client.AuthenticateAsync("blondinkaTest1@gmail.com", "15926489z");
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }

        // Форма регистрации нового пользователя по пригласительному токену, полученному по email
        public InviteResult Invite(string token)
        {
            InviteResult result = new InviteResult();
            try
            {
                Invite invite = _context.Invite.FirstOrDefault(i => i.Token == token);
                if (invite == null)
                {
                    return new InviteResult()
                    {
                        Status = ResultBase.StatusBad,
                        ErrorMessages = new List<ErrorMessageResult>() { new ErrorMessageResult() { message = "Пригласительный токен недействителен. Обратитесь к администратору компании для получения приглашения" } }
                    };
                }
                if (invite.ExpiresIn < DateTime.Now)
                {
                    _context.Invite.Remove(invite);
                    _context.SaveChanges();
                    return new InviteResult()
                    {
                        Status = ResultBase.StatusBad,
                        ErrorMessages = new List<ErrorMessageResult>() { new ErrorMessageResult() { message = "Пригласительный токен просрочен. Обратитесь к администратору компании для получения нового приглашения" } }
                    };
                }
                result.InviteToken = token;
                result.CompanyName = _context.Company
                                            .Where(c => c.Uid == invite.CompanyUid)
                                            .Select(c => c.CompanyName)
                                            .FirstOrDefault();
                result.Status = ResultBase.StatusOk;
                return result;
            }
            catch (Exception ex)
            {
                return ErrorHandler<InviteResult>.SetDBProblem(result, ex.Message);
            }
        }

        //Регистрация нового пользователя по пригласительному токену, отправленному на имейл
        public Registration_Out CreateUserByInvite(Registration_In value)
        {
            Registration_Out result = new Registration_Out();

            try
            {
                if (string.IsNullOrEmpty(value.Login) || string.IsNullOrEmpty(value.Password))
                {
                    return ErrorHandler<Registration_Out>.SetDBProblem(result, "Логин или пароль не указан");
                }

                if (value.Login.Length > 50)
                {
                    return ErrorHandler<Registration_Out>.SetDBProblem(result, "Логин слишком длинный");
                }

                if (value.Password.Length < 6)
                {
                    return ErrorHandler<Registration_Out>.SetDBProblem(result, "Пароль слишком короткий");
                }

                Tuple<string, byte[]> keyPair = HelperSecurity.CreateKeyPair();
                string privateKey = keyPair.Item1;
                byte[] publicKey = keyPair.Item2;

                _context.LoadStoredProc("prRegistration")
                    .AddParam("Login", value.Login)
                    .AddParam("Password", value.Password)
                    .AddParam("InvitingToken", value.InvitingToken)
                    .AddParam("Name", value.Name)
                    .AddParam("Birthday", value.Birthday)
                    .AddParam("Phone", value.Phone)
                    .AddParam("Email", value.Email)
                    .AddParam("PublicKey", publicKey)
                    .AddParam("UserGuid", out IOutParam<Guid> outret)
                    .ReturnValue(out IOutParam<int> retparam)
                    .ExecNonQuery();

                switch (retparam.Value)
                {
                    case 6:
                        {
                            return ErrorHandler<Registration_Out>.SetDBProblem(result, "Пользователь с таким логином уже существует");
                        }
                    case 7:
                        {
                            return ErrorHandler<Registration_Out>.SetDBProblem(result, "Приглашение не действительно. Обратитесь к администратору");
                        }
                    case 8:
                        {
                            return ErrorHandler<Registration_Out>.SetDBProblem(result, "Ошибка");
                        }
                }

                result.PrivateKey = privateKey;
                var authEmp = (from e in _context.Employee
                               join rLeft in _context.Role on e.RoleUid equals rLeft.Uid into rTemp
                               from r in rTemp.DefaultIfEmpty()
                               where e.Uid == outret.Value
                               select new
                               {
                                   companyUid = e.CompanyUid,
                                   RoleName = r.RoleName
                               }).FirstOrDefault();

                Authorization_Out_FromDB auth = new Authorization_Out_FromDB()
                {
                    CompanyUID = authEmp.companyUid,
                    EmployeeUid = outret.Value,
                    RoleName = authEmp.RoleName
                };
                result.JWT = Helpers.HelperSecurity.GenerateToken(auth);
                result.UserUid = outret.Value;
                result.Status = ResultBase.StatusOk;
                return result;
            }
            catch (Exception ex)
            {
                return ErrorHandler<Registration_Out>.SetDBProblem(result, ex.Message);
            }
        }

        #endregion
// -----------------------------------------------------------------------------------------------------------------------------------------

        public GetMainPage_Out GetMainPage(string token)
        {
            GetMainPage_Out result = new GetMainPage_Out();

            try
            {
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
            }
            catch (Exception ex)
            {
                return ErrorHandler<GetMainPage_Out>.SetDBProblem(result, ex.Message);
            }
            return result;
        }

// --------------------------------------------------    ДЕЛО      -------------------------------------------------------------------------
        #region ДЕЛО

        public GetCasesList_Out GetCasesList(string token)
        {
            GetCasesList_Out result = new GetCasesList_Out();
            try
            {
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
            }
            catch (Exception ex)
            {
                return ErrorHandler<GetCasesList_Out>.SetDBProblem(result, ex.Message);
            }
            return result;
        }

        // добавление фигуранта к делу
        public ResultBase AddNewFigurantToCase(string token, NewCase_In figurantIn, Guid caseUid, string privateKey)
        {
            ResultBase result = new ResultBase();
            try
            {
                EmployeeCaseInfo employeeCaseInfo = HelperSecurity.GetEmployeeCaseInfo(token, caseUid, _context);
                if(!employeeCaseInfo.isOwner && employeeCaseInfo.userRole != "director")
                    return ErrorHandler<ResultBase>.SetDBProblem(result, "Недостаточно прав для добавления фигуранта дела");

                byte[] aesKey = HelperSecurity.DecryptByRSA(privateKey, employeeCaseInfo.encriptedAesKey);

                NewCase_Figurant_In figurant = figurantIn.Figurants.First();
                Figurant newFigurant = new Figurant()
                {
                    CaseUid = caseUid,
                    Email = figurant.Email,
                    FigurantRoleUid = figurant.RoleUid,
                    Name = figurant.Name,
                    Surname = figurant.Surname,
                    SecondName = figurant.SecondName,
                    Phone = figurant.Phone,
                    Description = HelperSecurity.EncryptByAes(string.IsNullOrEmpty(figurant.Description) ? "" : figurant.Description, aesKey)
                };
                _context.Figurant.Add(newFigurant);
                _context.SaveChanges();
                result.Status = ResultBase.StatusOk;
            }
            catch (Exception ex)
            {
                return ErrorHandler<ResultBase>.SetDBProblem(result, ex.Message);
            }
            return result;
        }     

        // Удаление фигуранта дела
        public ResultBase RemoveFigurantFromCase(string token, Guid caseUid, Guid figurantUid)
        {
            ResultBase result = new ResultBase();
            try
            {
                JWTClaims JWTValues = HelperSecurity.GetJWTClaimsValues(token);
                Guid userUidFromToken = JWTValues.employeeUid;
                string userRole = JWTValues.role;

                bool isOwner = _context.EmployeeCase
                                    .Where(e => e.EmployeeUid == userUidFromToken && e.CaseUid == caseUid)
                                    .Select(e =>  e.IsOwner)
                                    .FirstOrDefault();

                if (!isOwner && userRole != "director")
                    return ErrorHandler<ResultBase>.SetDBProblem(result, "Недостаточно прав для удаления фигуранта дела");

                Figurant figurant = _context.Figurant.FirstOrDefault(f => f.CaseUid == caseUid && f.Uid == figurantUid);
                if (figurant != null)
                {
                    _context.Figurant.Remove(figurant);
                    _context.SaveChanges();
                }
                result.Status = ResultBase.StatusOk;
            }
            catch (Exception ex)
            {
                return ErrorHandler<ResultBase>.SetDBProblem(result, ex.Message);
            }
            return result;
        }

        #endregion
// -----------------------------------------------------------------------------------------------------------------------------------------
    }
}
