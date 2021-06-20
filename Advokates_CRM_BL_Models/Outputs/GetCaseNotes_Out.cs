using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Advokates_CRM_DTO.Outputs
{
    public class GetCaseNotes_Out : ResultBase
    {
        public Case_Note[] Notes { get; set; }
    }
}
