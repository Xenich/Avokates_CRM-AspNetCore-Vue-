using Advokates_CRM.BL.Helpers;
using Advokates_CRM.BL.Objects;
using Advokates_CRM.DB.Models;
using Advokates_CRM.Layer_Interfaces;
using Advokates_CRM_DTO.Inputs;
using Advokates_CRM_DTO.Outputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Advokates_CRM.BL
{
    public class DataLayerFigurant :  DataLayerBase, IDataLayerFigurant
    {
        public DataLayerFigurant(LawyerCRMContext context) : base(context)
        {
        }

        // добавление фигуранта к делу
        public ResultBase AddNewFigurantToCase(string token, NewCase_In figurantIn, Guid caseUid, string privateKey)
        {
            ResultBase result = new ResultBase();
            EmployeeCaseInfo employeeCaseInfo = HelperSecurity.GetEmployeeCaseInfo(token, caseUid, _context);
            if (!employeeCaseInfo.isOwner && employeeCaseInfo.userRole != "director")
                return ErrorHandler<ResultBase>.SetDBProblem(result, "Недостаточно прав для добавления фигуранта дела");

            byte[] aesKey = HelperSecurity.DecryptByRSA(privateKey, employeeCaseInfo.encriptedAesKey);

            NewCase_Figurant_In figurant = figurantIn.Figurants.First();
            Figurant newFigurant = new Figurant()
            {
                CaseUid = caseUid,
                Email = figurant.Email,
                FigurantRoleUid = figurant.RoleUid,
                Name = figurant.Name,
                Surname = figurant.Surname,
                SecondName = figurant.SecondName,
                Phone = figurant.Phone,
                Description = HelperSecurity.EncryptByAes(string.IsNullOrEmpty(figurant.Description) ? "" : figurant.Description, aesKey)
            };
            _context.Figurant.Add(newFigurant);
            _context.SaveChanges();
         
            result.Status = ResultBase.StatusOk;
            return result;
        }

        // Удаление фигуранта дела
        public ResultBase RemoveFigurantFromCase(string token, Guid caseUid, Guid figurantUid)
        {
            ResultBase result = new ResultBase();

            JWTClaims JWTValues = HelperSecurity.GetJWTClaimsValues(token);
            Guid userUidFromToken = JWTValues.employeeUid;
            string userRole = JWTValues.role;

            bool isOwner = _context.EmployeeCase
                                .Where(e => e.EmployeeUid == userUidFromToken && e.CaseUid == caseUid)
                                .Select(e => e.IsOwner)
                                .FirstOrDefault();

            if (!isOwner && userRole != "director")
                return ErrorHandler<ResultBase>.SetDBProblem(result, "Недостаточно прав для удаления фигуранта дела");

            Figurant figurant = _context.Figurant.FirstOrDefault(f => f.CaseUid == caseUid && f.Uid == figurantUid);
            if (figurant != null)
            {
                _context.Figurant.Remove(figurant);
                _context.SaveChanges();
            }
         
            result.Status = ResultBase.StatusOk;
            return result;
        }
    }
}
