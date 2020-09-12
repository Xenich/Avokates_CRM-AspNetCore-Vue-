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

// -------------------------    НОВЫЙ ПОЛЬЗОВАТЕЛЬ СИСТЕМЫ      -------------------------
#region Новый пользователь системы
        
        public  ResultBase CreateInvite(string token, string email)
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
                    ErrorHandler<ResultBase>.SetDBProblem(result,"Email not valid");
                    return result;
                }
                SHA256 sha256 = SHA256.Create();
                byte[] bytes = Encoding.UTF8.GetBytes(email);
                string inviteToken = Convert.ToBase64String( sha256.ComputeHash(bytes));

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

                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(1024);
                rsa.PersistKeyInCsp = false;
                byte[] publicKey = rsa.ExportCspBlob(false);
                byte[] privteKey = rsa.ExportCspBlob(true);

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

                result.PrivateKey = Convert.ToBase64String(privteKey);
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
//---------------------------------------------------------------------------------------

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
                             userName = ((string.IsNullOrEmpty(e.Surname)?"": e.Surname) + " " + (string.IsNullOrEmpty(e.Name) ? "" : e.Name) + " "
                             + (string.IsNullOrEmpty(e.SecondName) ? "" : e.SecondName)),
                             companyName = c.CompanyName
                         }).FirstOrDefault();

                result.Name = r.userName;
                result.CompanyName = r.companyName;


                result.CasesCount = (from  ec in _context.EmployeeCase
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
                              ).FirstOrDefault(),
                         Uid = c.Uid
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
                //Dictionary<string, string> jwtValues = HelperSecurity.GetJWTClaimsValues(token);
                //int companyId = int.Parse(jwtValues["companyId"]);

                Guid companyGuid = HelperSecurity.GetCompanyUidByJWT(token);


                // var r = from 
                Case _case = (from c in _context.Case
                              join cm in _context.Company on c.CompanyUid equals cm.Uid
                              where cm.Uid == companyGuid && c.IdPerCompany == caseId && !c.IsClosed
                              select c).FirstOrDefault();

                result.Title = _case.Title;
            }
            catch (Exception ex)
            {
                ErrorHandler<GetCase_Out>.SetDBProblem(result, ex.Message);
            }
            return result;
        }

        public NewCaseGetModel_Out NewCaseGetModel()
        {
            NewCaseGetModel_Out result = new NewCaseGetModel_Out();

            try 
            {
                result.FigurantRoleOptions = _context.FigurantRole.Select(f => new ItemView()
                {
                    Id = f.Uid.ToString(),
                    Name = f.RoleName
                })
                .OrderBy(f=>f.Name)
                .ToArray();
            }
            catch (Exception ex)
            {
                ErrorHandler<NewCaseGetModel_Out>.SetDBProblem(result, ex.Message);
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
                              Role = r.RoleName,
                               UserUid = e.Uid
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

        public ResultBase CreateNewCase(NewCase_In inputValue)
        {
            ResultBase result = new ResultBase();
            try
            {
            }
            catch (Exception ex)
            {
                ErrorHandler<ResultBase>.SetDBProblem(result, ex.Message);
            }
            return result;
        }

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
