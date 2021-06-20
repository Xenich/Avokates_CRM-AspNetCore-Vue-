using Advokates_CRM.BL.Helpers;
using Advokates_CRM.BL.Objects;
using Advokates_CRM.DB.Models;
using Advokates_CRM.Layer_Interfaces;
using Advokates_CRM_DTO.Inputs;
using Advokates_CRM_DTO.Outputs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Advokates_CRM.BL
{
    public class DataLayerNote : DataLayerBase, IDataLayerNote
    {
        public DataLayerNote(LawyerCRMContext context) : base(context)
        {
        }

        // добавление новой записи к делу
        public ResultBase AddNewNoteToCase(string token, NewNote_In note, IFormFile[] files, Guid caseUid, string privateKey)
        {
            ResultBase result = new ResultBase();

            EmployeeCaseInfo employeeCaseInfo = HelperSecurity.GetEmployeeCaseInfo(token, caseUid, _context);
            //if (!employeeCaseInfo.isOwner && employeeCaseInfo.userRole != "director")
            //    return ErrorHandler<ResultBase>.SetDBProblem(result, "Недостаточно прав для добавления записи по делу");
            bool employeeAccessExists = _context.EmployeeCase
                            .Where(e => e.EmployeeUid == employeeCaseInfo.employeeGuid && e.CaseUid == caseUid)
                            .Any();
            if (!employeeAccessExists)
            {
                return ErrorHandler<ResultBase>.SetDBProblem(result, "Недостаточно прав для осуществления данной операции");
            }

            byte[] aesKey = HelperSecurity.DecryptByRSA(privateKey, employeeCaseInfo.encriptedAesKey);
            DateTime date = DateTime.Now;
            Note newNote = new Note()
            {
                CaseUid = caseUid,
                EmployeeUid = employeeCaseInfo.employeeGuid,
                Text = HelperSecurity.EncryptByAes(string.IsNullOrEmpty(note.Text) ? "" : note.Text, aesKey),
                Date = date,
                Updatedate = date,
                Title = HelperSecurity.EncryptByAes(string.IsNullOrEmpty(note.Title) ? "" : note.Title, aesKey)
            };
            _context.Note.Add(newNote);
            _context.SaveChanges();

            string filesDirectory = Directory.GetCurrentDirectory() + "\\wwwroot\\" + "Files";
            if (!Directory.Exists(filesDirectory))
                Directory.CreateDirectory(filesDirectory);



            for (int i = 0; i < files.Length; i++)
            {
                IFormFile file = files[i];
                Guid guid = Guid.NewGuid();
                string path = filesDirectory + "\\" + guid;

                HelperSecurity.WriteToEncriptedFile(file, aesKey, path);

                MediaFile mediaFile = new MediaFile()
                {
                    Uid = guid,
                    NoteUid = newNote.Uid,
                    FilePath = "Files" + "\\" + guid,
                    NameCripted = HelperSecurity.EncryptByAes(file.FileName, aesKey)
                };
                _context.MediaFile.Add(mediaFile);
            }
            _context.SaveChanges();

            return result;
        }

        public ResultBase RemoveNoteFromCase(string token, Guid caseUid, Guid noteUid)
        {
            ResultBase result = new ResultBase();
            EmployeeCaseInfo employeeCaseInfo = HelperSecurity.GetEmployeeCaseInfo(token, caseUid, _context);
            if (employeeCaseInfo == null)
                return ErrorHandler<ResultBase>.SetDBProblem(result, "Нет доступа");
            bool canDelete = false;

            var noteInfo = (from n in _context.Note
                            join c in _context.Case on n.CaseUid equals c.Uid
                            where n.CaseUid == caseUid && n.Uid == noteUid && c.CompanyUid == employeeCaseInfo.companyUid
                            select new
                            {
                                employeeUid = n.EmployeeUid,
                                companyUid = c.CompanyUid,
                                caseOwner = _context.EmployeeCase
                                                    .Where(c => c.CaseUid == caseUid && c.IsOwner)
                                                    .Select(e => e.EmployeeUid)
                                                    .FirstOrDefault(),
                                isDirectorNote = (from e in _context.Employee
                                                  join r in _context.Role on e.RoleUid equals r.Uid
                                                  where e.Uid == n.EmployeeUid
                                                  select r.RoleName).FirstOrDefault() == "director"
                            }).FirstOrDefault();

            if (noteInfo == null)
                return ErrorHandler<ResultBase>.SetDBProblem(result, "Запись не найдена");

            if (employeeCaseInfo.userRole == "director")
                canDelete = true;
            else
            {
                if (noteInfo.caseOwner == employeeCaseInfo.employeeGuid)
                {
                    if (noteInfo.isDirectorNote)
                        return ErrorHandler<ResultBase>.SetDBProblem(result, "Вы не можете удалить запись, сделанную директором компании");
                    else
                        canDelete = true;
                }
                else
                {
                    if (noteInfo.employeeUid == employeeCaseInfo.employeeGuid)
                        canDelete = true;
                    else
                        return ErrorHandler<ResultBase>.SetDBProblem(result, "Нет прав на удаление данной записи");
                }
            }

            if (canDelete)
            {
                Note note = _context.Note.FirstOrDefault(n => n.Uid == noteUid);
                _context.Note.Remove(note);
                _context.SaveChanges();
            }
         
            result.Status = ResultBase.StatusOk;
            return result;
        }
    }
}
