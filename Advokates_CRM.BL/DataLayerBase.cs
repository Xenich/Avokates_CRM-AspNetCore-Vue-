using Advokates_CRM.DB.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Advokates_CRM.BL
{
    public class DataLayerBase
    {
        public DataLayerBase(LawyerCRMContext context)
        {
            _context = context;
        }
        protected readonly LawyerCRMContext _context;
    }
}
