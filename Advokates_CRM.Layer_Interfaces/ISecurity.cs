using Advokates_CRM_DTO.Outputs;
using Advokates_CRM_DTO.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Advokates_CRM.Layer_Interfaces
{
    public interface ISecurity
    {
        //ResultBase Registration(Registration_In reg);
        ResultBase Authorization(Authorization_In auth);
    }
}
