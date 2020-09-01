using System;
using System.Collections.Generic;

namespace Avokates_CRM.Models.DB
{
    public partial class Note
    {
        public Note()
        {
            MediaFile = new HashSet<MediaFile>();
        }

        public Guid Uid { get; set; }
        public int Id { get; set; }
        public Guid CaseUid { get; set; }
        public Guid EmployeeUid { get; set; }
        public string Text { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? Updatedate { get; set; }
        public string Title { get; set; }

        public Case CaseU { get; set; }
        public Employee EmployeeU { get; set; }
        public ICollection<MediaFile> MediaFile { get; set; }
    }
}
