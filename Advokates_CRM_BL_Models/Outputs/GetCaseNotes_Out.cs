using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Advokates_CRM_BL_Models.Outputs
{
    public class GetCaseNotes_Out : ResultBase
    {
        public Case_Note[] Notes { get; set; }
    }
}
