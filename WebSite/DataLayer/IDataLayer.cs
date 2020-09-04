using Avokates_CRM.Models.Outputs;
using Avokates_CRM.Models.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebSite.Models.Outputs;

namespace WebSite.DataLayer
{
    public interface IDataLayer
    {
        string CheckConnectionToDB();
        GetMainPage_Out GetMainPage(string token);

        GetCasesList_Out GetCasesList(string token);
        GetCase_Out GetCase(string token, int CaseId);
        GetCabinetInfo_Out GetCabinetInfo(string token);
        ResultBase CabinetInfoSaveChanges(string token, GetCabinetInfo_Out cabinetInfo);
        //ResultBase GetCases(BaseAuth_In inputValue);
        //ResultBase GetCaseNotes(BaseAuth_In inputValue);

        // ResultBase GetFigurantRoles( BaseAuth_In inputValue);
        //ResultBase CreateNewCase(NewCase_In inputValue);

    }
}
