using Advokates_CRM.BL.Helpers;
using Advokates_CRM.DB.Models;
using Advokates_CRM.Layer_Interfaces;
using Advokates_CRM_DTO.Inputs;
using Advokates_CRM_DTO.Outputs;
using StoredProcedureEFCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Advokates_CRM.BL
{
    public class DataLayerEmployee: DataLayerBase, IDataLayerEmployee
    {
        public DataLayerEmployee(LawyerCRMContext context) : base(context)
        {
        }

        // создание приглашения новому пользователю путём отсылки ему email с токеном приглашения
        public ResultBase CreateInvite(string token, string email)
        {
            ResultBase result = new ResultBase();
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
            MailSender.SendEmail(email, "Регистрация Advokates CRM", message);
            _context.SaveChanges();
            result.Status = ResultBase.StatusOk;

            return result;
        }

        // Форма регистрации нового пользователя по пригласительному токену, полученному по email
        public InviteResult Invite(string inviteToken)
        {
            InviteResult result = new InviteResult();

            Invite invite = _context.Invite.FirstOrDefault(i => i.Token == inviteToken);
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
            result.InviteToken = inviteToken;
            result.CompanyName = _context.Company
                                        .Where(c => c.Uid == invite.CompanyUid)
                                        .Select(c => c.CompanyName)
                                        .FirstOrDefault();
            result.Status = ResultBase.StatusOk;
            return result;
        }

        //Регистрация нового пользователя по пригласительному токену, отправленному на имейл
        public Registration_Out CreateUserByInvite(Registration_In value)
        {
            Registration_Out result = new Registration_Out();

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

        public string GetUserUidByJWT(string token)
        {
            return HelperSecurity.GetUserUidByJWT(token).ToString();
        }
    }
}
