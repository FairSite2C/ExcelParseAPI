using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ORMModel
{
    public class PersonMap
    {
        public string Name { get; set; }

        public bool Locked { get; set; }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public DateTime? CreateDT { get; set; }

        public DateTime? UpdateDT { get; set; }

        public long? CreateBy { get; set; }

        public long? UpdateBy { get; set; }

        public bool Deleted { get; set; }

        [ForeignKey("Person")]
        public long PersonId { get; set; }

        public Person Person { get; set; }

        [ForeignKey("MasterMap")]
        public long MasterMapId { get; set; }

        public MasterMap MasterMap { get; set; }

        public ICollection<PersonMapColumn> Columns { get; set; }

        public ICollection<Import> Imports { get; set; }
    }
}
