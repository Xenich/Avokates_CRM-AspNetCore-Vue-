using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Advokates_CRM_DTO.Outputs
{
    public class GetCaseNotes_Out : ResultBase
    {
        /// <summary>
        /// Общее количесмтво страниц
        /// </summary>
        public int PageCount { get; set; }
        /// <summary>
        /// Текущая страница
        /// </summary>
        public int CurrentPage { get; set; }
        public Case_Note[] Notes { get; set; }
    }
}
