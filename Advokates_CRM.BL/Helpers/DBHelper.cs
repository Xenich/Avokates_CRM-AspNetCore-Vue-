using Advokates_CRM.DB.Models;
using Advokates_CRM_DTO.Outputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Advokates_CRM.BL.Helpers
{
    internal class DBHelper
    {
        public static ItemView[] GetFigurantRoleOptions(Guid companyUid, LawyerCRMContext _context)
        {
            return _context.FigurantRole.Where(f => f.CompanyUid == companyUid).Select(f => new ItemView()
            {
                Id = f.Uid.ToString(),
                Name = f.RoleName
            })
            .OrderBy(f => f.Name)
            .ToArray();

        }

        /// <summary>
        /// Метод получения массива записей по делу
        /// </summary>
        /// <param name="caseUid">Guid дела в базе данных</param>
        /// <param name="symmetricKey">Симметричный ключ, связанный с делом</param>
        /// <param name="userRole">роль запрашивающего пользователя по делу</param>
        /// <param name="isOwner">переменная типа bool, указывающая, является пользователь, вызывающий данный метод создателем дела</param>
        /// <param name="userUidFromToken">Guid пользователя, вызывающего метод</param>
        /// <param name="_context">контекст базы данных</param>
        /// <returns>Массив объектов типа Case_Note</returns>
        public static GetCaseNotes_Out GetCaseNotes(Guid caseUid, byte[] symmetricKey, string userRole, bool isOwner, Guid userUidFromToken, int elementsCount, int currentPage, LawyerCRMContext _context)
        {

            int skip = PaginationHelper.CalculateOffset(currentPage, elementsCount);

            IQueryable<Case_Note> query = from n in _context.Note
                    join e in _context.Employee on n.EmployeeUid equals e.Uid
                    where n.CaseUid == caseUid
                    orderby n.Updatedate descending
                    select new Case_Note()
                    {
                        Id = n.Id,
                        Uid = n.Uid,
                        Date = n.Updatedate.Value.ToShortDateString() + " " + n.Updatedate.Value.ToShortTimeString(),
                        EmployeeInfo = (string.IsNullOrEmpty(e.Surname) ? "" : e.Surname) + " " + (string.IsNullOrEmpty(e.Name) ? "" : e.Name) + " " + (string.IsNullOrEmpty(e.SecondName) ? "" : e.SecondName),
                        Title = n.Title == null ? "" : HelperSecurity.DecriptByAes(n.Title, symmetricKey),
                        Text = n.Text == null ? "" : HelperSecurity.DecriptByAes(n.Text, symmetricKey),
                        CanDelete = (userRole == "director" || isOwner) || n.EmployeeUid == userUidFromToken
                    };

            int count = query.Count();

            GetCaseNotes_Out result = new GetCaseNotes_Out();

            result.CurrentPage = currentPage;
            result.PageCount = (int)Math.Ceiling((decimal)count / elementsCount);
            result.Notes = query.Skip(skip).Take(elementsCount).ToArray();

            return result;
        }
    }
}
