using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ORMModel
{
    [Table("MasterMap")]
    public class MasterMap
    {
        public string Name { get; set; }

        public string TableName { get; set; }

        public bool Locked { get; set; }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public DateTime? CreateDT { get; set; }

        public DateTime? UpdateDT { get; set; }

        public long? CreateBy { get; set; }

        public long? UpdateBy { get; set; }

        public bool Deleted { get; set; }

        public ICollection<MasterMapColumn> Columns { get; set; }

        public ICollection<PersonMap> PersonMaps { get; set; }

    }
}
