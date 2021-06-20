using Advokates_CRM_DTO.Outputs;
using Advokates_CRM_DTO.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Advokates_CRM.Layer_Interfaces
{
    public interface IDataLayer
    {
        string CheckConnectionToDB();

        GetMainPage_Out GetMainPage(string token);

        GetCasesList_Out GetCasesList(string token);

    }
}
