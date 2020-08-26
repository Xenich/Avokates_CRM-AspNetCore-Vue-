using Avokates_CRM.Models.Outputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebSite.Models.Outputs
{
    public class GetCasesList_Out:ResultBase
    {
        public IEnumerable<Case_Out> CasesList { get; set; }
    }

    public class Case_Out
    {
        public string CaseTitle { get; set; }
        public string CaseOwner { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int IdPerCompany { get; set; }
        public Guid Uid { get; set; }
    }
}
