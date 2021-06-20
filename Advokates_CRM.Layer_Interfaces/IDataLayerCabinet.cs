using Advokates_CRM_DTO.Inputs;
using Advokates_CRM_DTO.Outputs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Advokates_CRM.Layer_Interfaces
{
    public interface IDataLayerCabinet
    {
        GetCabinetInfo_Out GetCabinetInfo(string token);
        ResultBase CabinetInfoSaveChanges(string token, CabinetInfo_In cabinetInfo);
    }
}
