using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ORMModel
{
    public class Import
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string FileName { get; set; }

        public long FileLength { get; set; }

        public int Status { get; set; }

        public string BlobName { get; set; }

        public int SkipRows { get; set; }

        public int WarnRows { get; set; }

        public DateTime? CreateDT { get; set; }

        public DateTime? UpdateDT { get; set; }

        public long? CreateBy { get; set; }

        public long? UpdateBy { get; set; }

        public bool Deleted { get; set; }

        [ForeignKey("Map")]
        public long PersonMapID { get; set; }

        public PersonMap Map { get; set; }
         
        public ICollection<ImportError> ImportErrors { get; set; }

        public ICollection<Sale> Sales { get; set; }
    }
}
