using Avokates_CRM.Models.Outputs;
using Avokates_CRM.Models.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebSite.DataLayer
{
    public interface ISecuryty
    {
        //ResultBase Registration(Registration_In reg);
        ResultBase Authorization(Authorization_In auth);
    }
}
