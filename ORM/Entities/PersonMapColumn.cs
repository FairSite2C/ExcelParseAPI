using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ORMModel
{
    public class PersonMapColumn
    {
        public string OurHeader { get; set; }

        public string TheirHeader { get; set; }

        public string Regex { get; set; }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public DateTime? CreateDT { get; set; }

        public DateTime? UpdateDT { get; set; }

        public long? CreateBy { get; set; }

        public long? UpdateBy { get; set; }

        public bool Deleted { get; set; }

        [ForeignKey("Map")]
        public long? PersonMapID { get; set; }

        public PersonMap Map { get; set; }

    }
}
