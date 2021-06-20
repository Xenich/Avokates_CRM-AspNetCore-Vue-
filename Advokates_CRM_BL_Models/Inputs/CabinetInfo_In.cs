using System;
using System.Collections.Generic;
using System.Text;

namespace Advokates_CRM_DTO.Inputs
{
    public class CabinetInfo_In
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string SecondName { get; set; }
        public string PublicKey { get; set; }
        public string Role { get; set; }
        public string Birthday { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public Guid UserUid { get; set; }
    }
}
