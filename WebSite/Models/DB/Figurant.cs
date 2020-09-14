﻿using System;
using System.Collections.Generic;

namespace Avokates_CRM.Models.DB
{
    public partial class Figurant
    {
        public Guid Uid { get; set; }
        public Guid CaseUid { get; set; }
        public Guid? FigurantRoleUid { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string FigurantRoleName { get; set; }
        public string SecondName { get; set; }
        public string Surname { get; set; }
        public byte[] Description { get; set; }

        public Case CaseU { get; set; }
        public FigurantRole FigurantRoleU { get; set; }
    }
}
