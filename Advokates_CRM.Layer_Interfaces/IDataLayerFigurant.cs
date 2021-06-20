using Advokates_CRM_DTO.Inputs;
using Advokates_CRM_DTO.Outputs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Advokates_CRM.Layer_Interfaces
{
    public interface IDataLayerFigurant
    {
        ResultBase AddNewFigurantToCase(string token, NewCase_In figurantIn, Guid caseUid, string privateKey);
        ResultBase RemoveFigurantFromCase(string token, Guid caseUid, Guid figurantUid);
    }
}
