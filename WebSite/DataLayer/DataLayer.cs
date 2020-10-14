using Avokates_CRM.Models.Outputs;
using Avokates_CRM;

using Avokates_CRM.Models.DB;
using Avokates_CRM.Models.Inputs;
using StoredProcedureEFCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MimeKit;
using MailKit.Net.Smtp;

using WebSite.Helpers;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using Avokates_CRM.Helpers;
using System.Security.Cryptography;
using Avokates_CRM.Models.ApiModels;

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

        public GetCase_Out GetCase(string token, int caseId, string privateKey)
        {

            GetCase_Out result = new GetCase_Out();
            try
            {
                //string userUidFromToken = HelperSecurity.GetUserUidByJWT(token);
                //Dictionary<string, string> jwtValues = HelperSecurity.GetJWTClaimsValues(token);
                //int companyId = int.Parse(jwtValues["companyId"]);

                JWTClaims JWTValues = HelperSecurity.GetJWTClaimsValues(token);
                Guid userUidFromToken = JWTValues.employeeUid;
                Guid companyUidFromToken = JWTValues.companyUid;
                string userRole = JWTValues.role;



                EmployeeCase employeeCase = (from EmployeeCase ec in _context.EmployeeCase
                                             join c in _context.Case on ec.CaseUid equals c.Uid
                                             where userRole == "director" ? (c.CompanyUid == companyUidFromToken && c.IdPerCompany == caseId && ec.EmployeeUid == userUidFromToken) :
                                                                          (c.CompanyUid == companyUidFromToken && c.IdPerCompany == caseId && ec.EmployeeUid == userUidFromToken && !c.IsClosed)
                                             select ec)
                                             .FirstOrDefault();
                if (employeeCase == null)
                    return ErrorHandler<GetCase_Out>.SetDBProblem(result, "Нет права доступа. Обратитесь к администратору");

                byte[] symmetricKey = HelperSecurity.DecryptByRSA(privateKey, employeeCase.EncriptedAesKey);

                var _case = (from c in _context.Case
                             join cm in _context.Company on c.CompanyUid equals cm.Uid
                             where cm.Uid == companyUidFromToken && c.IdPerCompany == caseId
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

                result.Notes = DBHelper.GetCaseNotes(_case.UID, symmetricKey, userRole, employeeCase.IsOwner, userUidFromToken, _context);
                //(from n in _context.Note
                //                join e in _context.Employee on n.EmployeeUid equals e.Uid
                //                where n.CaseUid == result.CaseUid
                //                orderby n.Updatedate descending
                //                select new Case_Note()
                //                {
                //                    Id = n.Id,
                //                    Uid = n.Uid,
                //                    Date = n.Updatedate.Value.ToShortDateString() + " " + n.Updatedate.Value.ToShortTimeString(),
                //                    EmployeeInfo = (string.IsNullOrEmpty(e.Surname) ? "" : e.Surname) + " " + (string.IsNullOrEmpty(e.Name) ? "" : e.Name) + " " + (string.IsNullOrEmpty(e.SecondName) ? "" : e.SecondName),
                //                    Title = n.Title == null ? "" : HelperSecurity.DecriptByAes(n.Title, symmetricKey),
                //                    Text = n.Text == null ? "" : HelperSecurity.DecriptByAes(n.Text, symmetricKey),
                //                    CanDelete = (userRole == "director" || employeeCase.IsOwner) || n.EmployeeUid == userUidFromToken
                //                }).ToArray();
                result.FigurantRoleOptions = DBHelper.GetFigurantRoleOptions(companyUidFromToken, _context);
                result.Status = ResultBase.StatusOk;

                //TODO : доделать метод
            }
            catch (Exception ex)
            {
                return ErrorHandler<GetCase_Out>.SetDBProblem(result, ex.Message);
            }
            return result;
        }

        // метод вызывается при обновлении списка записей (например при добавлении новой записи)
        public GetCaseNotes_Out GetCaseNotes(string token, Guid caseUid, string privateKey)
        {
            GetCaseNotes_Out result = new GetCaseNotes_Out();
            try
            {
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

                result.Notes = DBHelper.GetCaseNotes(caseUid, symmetricKey, userRole, employeeCase.IsOwner, userUidFromToken, _context);
                    //(from n in _context.Note
                    //            join e in _context.Employee on n.EmployeeUid equals e.Uid
                    //            where n.CaseUid == caseUid
                    //            orderby n.Updatedate descending
                    //            select new Case_Note()
                    //            {
                    //                Id = n.Id,
                    //                Uid = n.Uid,
                    //                Date = n.Updatedate.Value.ToShortDateString() + " " + n.Updatedate.Value.ToShortTimeString(),
                    //                EmployeeInfo = (string.IsNullOrEmpty(e.Surname) ? "" : e.Surname) + " " + (string.IsNullOrEmpty(e.Name) ? "" : e.Name) + " " + (string.IsNullOrEmpty(e.SecondName) ? "" : e.SecondName),
                    //                Title = n.Title == null ? "" : HelperSecurity.DecriptByAes(n.Title, symmetricKey),
                    //                Text = n.Text == null ? "" : HelperSecurity.DecriptByAes(n.Text, symmetricKey),
                    //                CanDelete = (userRole == "director" || employeeCase.IsOwner) || n.EmployeeUid == userUidFromToken
                    //            }).ToArray();

                result.Status = ResultBase.StatusOk;
            }
            catch (Exception ex)
            {
                return ErrorHandler<GetCaseNotes_Out>.SetDBProblem(result, ex.Message);
            }
            return result;
        }

        // добавление новой записи к делу
        public ResultBase AddNewNoteToCase(string token, NewNote_In note, Guid caseUid, string privateKey)
        {
            ResultBase result = new ResultBase();
            try
            {
                EmployeeCaseInfo employeeCaseInfo = HelperSecurity.GetEmployeeCaseInfo(token, caseUid, _context);
                //if (!employeeCaseInfo.isOwner && employeeCaseInfo.userRole != "director")
                //    return ErrorHandler<ResultBase>.SetDBProblem(result, "Недостаточно прав для добавления записи по делу");
                bool employeeAccessExists = _context.EmployeeCase
                                .Where(e => e.EmployeeUid == employeeCaseInfo.employeeGuid && e.CaseUid == caseUid)
                                .Any();
                if (!employeeAccessExists)
                {
                    return ErrorHandler<ResultBase>.SetDBProblem(result, "Недостаточно прав для осуществления данной операции");
                }

                byte[] aesKey = HelperSecurity.DecryptByRSA(privateKey, employeeCaseInfo.encriptedAesKey);
                DateTime date = DateTime.Now;
                Note newNote = new Note()
                {
                    CaseUid = caseUid,
                    EmployeeUid = employeeCaseInfo.employeeGuid,
                    Text = HelperSecurity.EncryptByAes(string.IsNullOrEmpty(note.Text) ? "" : note.Text, aesKey),
                    Date = date,
                    Updatedate = date,
                    Title = HelperSecurity.EncryptByAes(string.IsNullOrEmpty(note.Title) ? "" : note.Title, aesKey)
                };
                _context.Note.Add(newNote);
                _context.SaveChanges();
                result.Status = ResultBase.StatusOk;
            }
            catch (Exception ex)
            {
                return ErrorHandler<ResultBase>.SetDBProblem(result, ex.Message);
            }
            return result;
        }

        public ResultBase RemoveNoteFromCase(string token, Guid caseUid, Guid noteUid)
        {
            ResultBase result = new ResultBase();
            try
            {
                EmployeeCaseInfo employeeCaseInfo = HelperSecurity.GetEmployeeCaseInfo(token, caseUid, _context);
                if(employeeCaseInfo == null)
                    return ErrorHandler<ResultBase>.SetDBProblem(result, "Нет доступа");
                bool canDelete = false;

                var noteInfo = (from n in _context.Note
                               join c in _context.Case on n.CaseUid equals c.Uid
                               where n.CaseUid == caseUid && n.Uid == noteUid && c.CompanyUid == employeeCaseInfo.companyUid
                                select new                                   
                                {
                                    employeeUid = n.EmployeeUid,
                                    companyUid = c.CompanyUid,
                                    caseOwner = _context.EmployeeCase
                                                        .Where(c=>c.CaseUid == caseUid && c.IsOwner)
                                                        .Select(e=>e.EmployeeUid)
                                                        .FirstOrDefault(),
                                    isDirectorNote = (from e in _context.Employee
                                                     join r in _context.Role on e.RoleUid equals r.Uid
                                                     where e.Uid == n.EmployeeUid
                                                     select r.RoleName).FirstOrDefault() == "director"
                                }).FirstOrDefault();
                
                if(noteInfo==null)
                    return ErrorHandler<ResultBase>.SetDBProblem(result, "Запись не найдена");

                if (employeeCaseInfo.userRole == "director")
                    canDelete = true;
                else
                {
                    if (noteInfo.caseOwner == employeeCaseInfo.employeeGuid)
                    {
                        if (noteInfo.isDirectorNote)
                            return ErrorHandler<ResultBase>.SetDBProblem(result, "Вы не можете удалить запись, сделанную директором компании");
                        else
                            canDelete = true;
                    }
                    else
                    {
                        if (noteInfo.employeeUid == employeeCaseInfo.employeeGuid)
                            canDelete = true;
                        else
                            return ErrorHandler<ResultBase>.SetDBProblem(result, "Нет прав на удаление данной записи");
                    }
                }

                if (canDelete)
                {
                    Note note = _context.Note.FirstOrDefault(n => n.Uid == noteUid);
                    _context.Note.Remove(note);
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
            try
            {
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
                if(encriptedAesKey == null)
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
            }
            catch (Exception ex)
            {
                return ErrorHandler<ResultBase>.SetDBProblem(result, ex.Message);
            }
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
            try
            {
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
            }
            catch (Exception ex)
            {
                return ErrorHandler<ResultBase>.SetDBProblem(result, ex.Message);
            }
            return result;
        }

        public NewCaseGetModel_Out NewCaseGetModel(string token)
        {
            NewCaseGetModel_Out result = new NewCaseGetModel_Out();
            Guid companyUid = HelperSecurity.GetCompanyUidByJWT(token);
            try
            {
                result.FigurantRoleOptions = DBHelper.GetFigurantRoleOptions(companyUid, _context);
            }
            catch (Exception ex)
            {
                return ErrorHandler<NewCaseGetModel_Out>.SetDBProblem(result, ex.Message);
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

        // Создание нового дела
        public ResultBase CreateNewCase(NewCase_In inputValue)
        {
            ResultBase result = new ResultBase();
            try
            {
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

                // TODO : доделать метод
            }
            catch (Exception ex)
            {
                return ErrorHandler<ResultBase>.SetDBProblem(result, ex.Message);
            }
            return result;
        }
        #endregion
// -----------------------------------------------------------------------------------------------------------------------------------------

// --------------------------------------------------    Личный кабинет      ---------------------------------------------------------------
        #region Личный кабинет
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
                              PublicKey = e.PublicKey == null ? "" : Convert.ToBase64String(e.PublicKey),
                              Birthday = e.Birthday.Value.Date.ToString(),
                              Email = e.Email,
                              Phone = e.Phone,
                              Role = r.RoleName,
                              UserUid = e.Uid
                          }).FirstOrDefault();
                result.Status = ResultBase.StatusOk;

            }
            catch (Exception ex)
            {
                result = new GetCabinetInfo_Out();
                return ErrorHandler<GetCabinetInfo_Out>.SetDBProblem(result, ex.Message);
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
                bool bDaySuccessParse = DateTime.TryParse(cabinetInfo.Birthday, out bDay);
                if (bDaySuccessParse)
                    emp.Birthday = bDay;

                _context.SaveChanges();
                result.Status = ResultBase.StatusBad;
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
