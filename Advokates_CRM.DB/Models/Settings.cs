using System;
using System.Collections.Generic;

namespace Advokates_CRM.DB.Models
{
    public partial class Settings
    {
        public string Parameter { get; set; }
        public string Value { get; set; }
        public int Id { get; set; }
    }
}
