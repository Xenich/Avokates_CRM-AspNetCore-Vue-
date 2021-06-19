using Advokates_CRM_BL_Models.Outputs;
using Advokates_CRM_BL_Models.Inputs;
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

        ResultBase CreateInvite(string token, string email);
        InviteResult Invite(string inviteToken);

        GetMainPage_Out GetMainPage(string token);

        GetCasesList_Out GetCasesList(string token);
        ResultBase CabinetInfoSaveChanges(string token, GetCabinetInfo_Out cabinetInfo);

        GetCase_Out GetCase(string token, int caseId, string privateKey);
        GetCabinetInfo_Out GetCabinetInfo(string token);
        NewCaseGetModel_Out NewCaseGetModel(string token);
        ResultBase AddNewFigurantToCase(string token, NewCase_In figurantIn, Guid caseUid, string privateKey);
        ResultBase AddNewNoteToCase(string token, NewNote_In note, IFormFile[] files, Guid caseUid, string privateKey);
        ResultBase RemoveNoteFromCase(string token, Guid caseUid, Guid noteUid);
        GetCaseNotes_Out GetCaseNotes (string token, Guid caseUid, string privateKey);
        ResultBase RemoveFigurantFromCase(string token, Guid caseUid, Guid figurantUid);
        ResultBase GrantAccessToCase(string token, Guid userUid, Guid caseUid, string privateKey);
        ResultBase RemoveAccessToCase(string token, Guid userUid, Guid caseUid);
        

        Registration_Out CreateUserByInvite(Registration_In value);


        //ResultBase GetCases(BaseAuth_In inputValue);
        //ResultBase GetCaseNotes(BaseAuth_In inputValue);

        // ResultBase GetFigurantRoles( BaseAuth_In inputValue);
        ResultBase CreateNewCase(NewCase_In inputValue);

    }
}
