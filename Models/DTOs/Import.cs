using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace OriginsRx.Models.DTOs
{
    public class ImportPost
    {
        public long PersonMapId { get; set; }

        public long PersonId { get; set; }

        public string FileName { get; set; }

        public long FileLength { get; set; }

        public long ImportId { get; set; }

        [JsonIgnore]
        public MemoryStream Stream { get; set; }

        string BlobName { get; set; }

        public int TotalRows { get { return SkipRows + WarnRows; } }

        public int SkipRows { get; set; }

        public int WarnRows { get; set; }

        [JsonIgnore]
        public int SheetNo { get; set; }
    }

    public class Import
    {
        public long Id { get; set; }

        public long PersonMapID { get; set; }

        public string FileName { get; set; }

        string BlobName { get; set; }

        public long TotalRows { get { return SkipRows + WarnRows; } }

        public long SkipRows { get; set; }

        public long WarnRows { get; set; }

        public DateTime? CreateDT { get; set; }

        public DateTime? UpdateDT { get; set; }

        public long? CreateBy { get; set; }

        public long? UpdateBy { get; set; }

        public bool? Deleted { get; set; }
         
        public ICollection<ImportError> ImportErrors { get; set; }

//        public ICollection<Sale> Sales { get; set; }

    }
}
