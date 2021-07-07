using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Advokates_CRM.BL.Helpers
{
    public interface INumerable
    {
        /// <summary>
        /// Порядковий номер
        /// </summary>
        int NumberPP { get; set; }
    }

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

    class PaginationHelper
    {
        // рассчёт skip-а при пагинации
        public static int CalculateOffset(int pageNumber, int rows)
        {
            int offset = 0;
            if (pageNumber < 1)
                pageNumber = 1;

            if (rows < 1)
                rows = 20;
            offset = (pageNumber - 1) * rows;
            return offset;
        }

        /// <summary>
        /// Рассчёт количества страниц в запросе исходя из количества строк на странице
        /// </summary>
        /// <typeparam name="T">тип IQueryable</typeparam>
        /// <param name="query">Запрос IQueryable</param>
        /// <param name="itemOnPageCount">Количество строк на странице</param>
        /// <returns></returns>
        public static int GetRequestPagesCount<T>(IQueryable<T> query, int itemOnPageCount)
        {
            int count = query.Count();
            return (int)Math.Ceiling((decimal)count / itemOnPageCount);
        }

        /// <summary>
        /// Присвоение порядковых номеров элементам списка
        /// </summary>
        /// <param name="values"></param>
        public static void PopulateWithNumbers(IEnumerable<INumerable> values, int startValue = 0)
        {
            int counter = startValue;
            foreach (INumerable value in values)
            {
                value.NumberPP = ++counter;
            }
        }
    }
}
