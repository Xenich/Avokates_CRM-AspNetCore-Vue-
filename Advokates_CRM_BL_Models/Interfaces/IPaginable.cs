using System;
using System.Collections.Generic;
using System.Text;

namespace Advokates_CRM.DTO.Interfaces
{
    public interface IPaginable
    {
        /// <summary>
        /// Количество страниц выборки
        /// </summary>
        int PageCount { get; set; }
        /// <summary>
        /// Текущая страница
        /// </summary>
        int CurrentPage { get; set; }
    }
}
