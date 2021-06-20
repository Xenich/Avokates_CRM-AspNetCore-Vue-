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

        ResultBase CreateInvite(string token, string email);
        InviteResult Invite(string inviteToken);

        GetMainPage_Out GetMainPage(string token);

        GetCasesList_Out GetCasesList(string token);

        ResultBase AddNewFigurantToCase(string token, NewCase_In figurantIn, Guid caseUid, string privateKey);
        ResultBase RemoveFigurantFromCase(string token, Guid caseUid, Guid figurantUid);
      

        Registration_Out CreateUserByInvite(Registration_In value);

        // ResultBase GetFigurantRoles( BaseAuth_In inputValue);
    }
}
