using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Advokates_CRM_DTO.Outputs
{
    public class GetMainPage_Out : ResultBase
    {
        public string CompanyName { get; set; }
        [Display(Name = "Вы вошли как:")]
        public string Name { get; set; }
        [Display(Name ="Количество дел")]
        public int CasesCount { get; set; }
        [Display(Name = "Количество записей")]
        public int NotesCount { get; set; }
        public bool isAdmin { get; set; }
    }
}
