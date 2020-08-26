using System;
using System.Collections.Generic;

namespace Avokates_CRM.Models.DB
{
    public partial class Invite
    {
        public Guid CompanyUid { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresIn { get; set; }

        public Company CompanyU { get; set; }
    }
}
