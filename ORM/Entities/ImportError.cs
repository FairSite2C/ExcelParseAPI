using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ORMModel
{
    public class ImportError
    {
        public int Status { get; set; }

        public long Sheet { get; set; }

        public long Row { get; set; }

        public string Errors { get; set; }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
         public long Id { get; set; }
                
        public DateTime? CreateDT { get; set; }

        public DateTime? UpdateDT { get; set; }

        public long? CreateBy { get; set; }

        public long? UpdateBy { get; set; }

        public bool Deleted { get; set; }

        [ForeignKey("Import")]
        public long ImportId { get; set; }

        public Import Import { get; set; }
    }
}
