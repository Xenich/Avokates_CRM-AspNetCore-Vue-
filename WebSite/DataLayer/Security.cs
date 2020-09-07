using Avokates_CRM.Models.DB;
using Avokates_CRM.Models.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StoredProcedureEFCore;

using WebSite.Helpers;
using Avokates_CRM;
using WebSite.Helpers;
using Avokates_CRM.Models.Outputs;

namespace WebSite.DataLayer
{
    public class Security : ISecuryty
    {
        private readonly LawyerCRMContext _context;
        public Security(LawyerCRMContext context)
        {
            _context = context;
        }
      

        // Авторизация по логину и паролю
        public ResultBase Authorization(Authorization_In auth)
        {
            Authorization_Out result = new Authorization_Out();
            Authorization_Out_FromDB fromDB = new Authorization_Out_FromDB();
            Employee emp = new Employee();

            try
            {
                if (auth.Login == "" || auth.Password == "")
                {
                    ResultBase res = new ResultBase();
                    res.ErrorMessages = new List<ErrorMessageResult>() { new ErrorMessageResult() { message = "Логин или пароль не указан" } };
                    res.Status = ResultBase.StatusBad;
                    return res;
                }

                _context.LoadStoredProc("prAuthorization")
                    .AddParam("Login", auth.Login)
                    .AddParam("Password", auth.Password)
                    .ReturnValue(out IOutParam<int> retparam)
                    .Exec(r => fromDB = r.SingleOrDefault<Authorization_Out_FromDB>());

                switch (retparam.Value)
                {
                    case 6:
                        {
                            ResultBase res = new ResultBase();
                            res.ErrorMessages = new List<ErrorMessageResult>() { new ErrorMessageResult() { message = "Неправильно указан логин или пароль" } };
                            res.Status = ResultBase.StatusBad;
                            return res;
                        }
                }

                if (fromDB.IsActive != true)
                {
                    ResultBase res = new ResultBase();
                    res.ErrorMessages = new List<ErrorMessageResult>() { new ErrorMessageResult() { message = "Пользователь с данным логином неактивен. Обратитесь к администратору" } };
                    res.Status = ResultBase.StatusBad;
                    return res;
                }

                result.Status = ResultBase.StatusOk;
                string jwt = HelperSecurity.GenerateToken(fromDB);
                result.Token = jwt;
                result.Name = fromDB.Name;
                return result;
            }
            catch (Exception ex)
            {
                return ErrorHandler<Authorization_Out>.SetDBProblem(result, ex.Message);
            }
        }

    }
}
