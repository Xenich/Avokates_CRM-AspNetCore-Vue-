using Advokates_CRM_BL_Models.Outputs;
using Advokates_CRM_BL_Models.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Advokates_CRM.Layer_Interfaces
{
    public interface ISecuryty
    {
        //ResultBase Registration(Registration_In reg);
        ResultBase Authorization(Authorization_In auth);
    }
}
