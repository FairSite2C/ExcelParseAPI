using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ORMModel
{

    public class Person
    {
        public string Name { get; set; }

        public string Email { get; set; }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public DateTime? CreateDT { get; set; }

        public DateTime? UpdateDT { get; set; }

        public long? CreateBy { get; set; }

        public long? UpdateBy { get; set; }

        public bool Deleted { get; set; }

        [ForeignKey("Company")]
        public long? CompanyId { get; set; }

        public virtual Company Company { get; set; }

        public virtual ICollection<PersonMap> Maps { get; set; }
    }
}
