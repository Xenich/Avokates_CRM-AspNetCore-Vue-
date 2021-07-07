using System;
using System.Linq;
using Advokates_CRM.BL.Helpers;
using Advokates_CRM.DB.Models;
using Advokates_CRM.Layer_Interfaces;
using Advokates_CRM_DTO.Inputs;
using Advokates_CRM_DTO.Outputs;

namespace Advokates_CRM.BL
{
    public class DataLayerCabinet: DataLayerBase, IDataLayerCabinet
    {
        public DataLayerCabinet(LawyerCRMContext context) : base(context)
        {
        }

        public GetCabinetInfo_Out GetCabinetInfo(string token)
        {
            Guid userUidFromToken = HelperSecurity.GetUserUidByJWT(token);
            GetCabinetInfo_Out result = (from e in _context.Employee
                      join rLeft in _context.Role on e.RoleUid equals rLeft.Uid into rTemp
                      from r in rTemp.DefaultIfEmpty()
                      where e.Uid == userUidFromToken
                      select new GetCabinetInfo_Out
                      {
                          Name = e.Name,
                          Surname = e.Surname,
                          SecondName = e.SecondName,
                          PublicKey = e.PublicKey == null ? "" : Convert.ToBase64String(e.PublicKey),
                          Birthday = e.Birthday.Value.Date.ToString(),
                          Email = e.Email,
                          Phone = e.Phone,
                          Role = r.RoleName,
                          UserUid = e.Uid
                      }).FirstOrDefault();

            return result;
        }

        public ResultBase CabinetInfoSaveChanges(string token, CabinetInfo_In cabinetInfo)
        {
            ResultBase result = new ResultBase();

            Guid userUidFromToken = HelperSecurity.GetUserUidByJWT(token);
            Employee emp = _context.Employee.FirstOrDefault(e => e.Uid == userUidFromToken);
            emp.Surname = cabinetInfo.Surname;
            emp.Name = cabinetInfo.Name;
            emp.SecondName = cabinetInfo.SecondName;
            emp.Phone = cabinetInfo.Phone;
            emp.Email = cabinetInfo.Email;
            

            DateTime bDay;
            bool bDaySuccessParse = DateTime.TryParse(cabinetInfo.Birthday, out bDay);
            if (bDaySuccessParse)
                emp.Birthday = bDay;

            _context.SaveChanges();

            return result;
        }
    }
}
