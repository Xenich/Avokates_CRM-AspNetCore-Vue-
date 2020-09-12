using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Avokates_CRM.Models.Inputs
{
    public class CreateInvite_In : BaseAuth_In
    {
        public string Email { get; set; }
    }
}
