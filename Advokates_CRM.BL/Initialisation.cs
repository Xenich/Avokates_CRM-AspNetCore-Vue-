using Advokates_CRM.BL.Helpers;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Advokates_CRM.BL
{
    public class BisnesLogicInitialisation
    {
        private static bool isInit = false;
        public static void Init(IConfiguration configuration)
        {
            if (!isInit)
            {
                HelperSecurity.Init(configuration);
                BaseHelper.Init(configuration);
                isInit = true;
            }
            else
            {
                    // TODO : логгировать
                throw new Exception("Бизнес-логика уже была инициализирована");
            }
        }
    }
}
