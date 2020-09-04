using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Avokates_CRM.Helpers
{
    public class BaseHelper
    {
        static IConfiguration Configuration;
        public static void Init(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static string GetHostAddress()
        {
            return Configuration.GetSection("HostAddress").Get<string>();
        }
    }
}
