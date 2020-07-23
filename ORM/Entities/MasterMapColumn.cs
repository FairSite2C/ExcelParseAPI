using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ORMModel
{
    [Table("MasterMapColumn")]
    public class MasterMapColumn
    {

        public string Header { get; set; }

        public string ColumnName { get; set; }

        public int ColumnDataType { get; set; }

        public bool Required { get; set; }

        public bool MakeIndex { get; set; }

        public int? MaxLength { get; set; }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public DateTime? CreateDT { get; set; }

        public DateTime? UpdateDT { get; set; }

        public long? CreateBy { get; set; }

        public long? UpdateBy { get; set; }

        public bool Deleted { get; set; }
         
        [ForeignKey("MasterMap")]
        public long MasterMapID { get; set; }

        public MasterMap MasterMap { get; set; }
    }
}
