using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Advokates_CRM_DTO.Inputs
{
    public class Registration_In
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string InvitingToken { get; set; }       // пригласительный токен отправленный на имейл

        public string Name { get; set; }
        public DateTime? Birthday { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

    }
}
