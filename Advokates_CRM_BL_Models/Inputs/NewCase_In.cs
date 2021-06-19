using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Advokates_CRM_BL_Models.Inputs
{
    public class NewCase_In : BaseAuth_In
    {
        public string Title { get; set; }
        public string Info { get; set; }
        public ICollection<NewCase_Figurant_In> Figurants { get; set; }
    }
}
