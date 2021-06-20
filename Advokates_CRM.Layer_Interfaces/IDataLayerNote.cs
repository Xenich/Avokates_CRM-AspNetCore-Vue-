using Advokates_CRM_DTO.Inputs;
using Advokates_CRM_DTO.Outputs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Advokates_CRM.Layer_Interfaces
{
    public interface IDataLayerNote
    {
        ResultBase AddNewNoteToCase(string token, NewNote_In note, IFormFile[] files, Guid caseUid, string privateKey);
        ResultBase RemoveNoteFromCase(string token, Guid caseUid, Guid noteUid);
    }
}
