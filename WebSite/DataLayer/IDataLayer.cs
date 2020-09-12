using Avokates_CRM.Models.Outputs;
using Avokates_CRM.Models.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebSite.DataLayer
{
    public interface IDataLayer
    {
        string CheckConnectionToDB();

        ResultBase CreateInvite(string token, string email);
        InviteResult Invite(string inviteToken);

        GetMainPage_Out GetMainPage(string token);

        GetCasesList_Out GetCasesList(string token);
        ResultBase CabinetInfoSaveChanges(string token, GetCabinetInfo_Out cabinetInfo);

        GetCase_Out GetCase(string token, int CaseId);
        GetCabinetInfo_Out GetCabinetInfo(string token);
        NewCaseGetModel_Out NewCaseGetModel();
        Registration_Out CreateUserByInvite(Registration_In value);


        //ResultBase GetCases(BaseAuth_In inputValue);
        //ResultBase GetCaseNotes(BaseAuth_In inputValue);

        // ResultBase GetFigurantRoles( BaseAuth_In inputValue);
        ResultBase CreateNewCase(NewCase_In inputValue);

    }
}
