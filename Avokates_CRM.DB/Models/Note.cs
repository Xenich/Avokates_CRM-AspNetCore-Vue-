using System;
using System.Collections.Generic;

namespace Avokates_CRM.DB.Models
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
        public byte[] Text { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? Updatedate { get; set; }
        public byte[] Title { get; set; }

        public virtual Case CaseU { get; set; }
        public virtual Employee EmployeeU { get; set; }
        public virtual ICollection<MediaFile> MediaFile { get; set; }
    }
}
