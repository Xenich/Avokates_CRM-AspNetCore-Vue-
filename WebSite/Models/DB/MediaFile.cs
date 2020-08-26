using System;
using System.Collections.Generic;

namespace Avokates_CRM.Models.DB
{
    public partial class MediaFile
    {
        public Guid Uid { get; set; }
        public int Id { get; set; }
        public string FilePath { get; set; }
        public Guid? NoteUid { get; set; }

        public Note NoteU { get; set; }
    }
}
