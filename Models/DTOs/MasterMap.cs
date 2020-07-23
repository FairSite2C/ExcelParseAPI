using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using System.ComponentModel.DataAnnotations;

namespace OriginsRx.Models.DTOs
{
    public class MasterMap
    {
        public string Name { get; set; }

        public string TableName { get; set; }

        public bool? Locked { get; set; }

        public long Id { get; set; }

        public DateTime? CreateDT { get; set; }

        public DateTime? UpdateDT { get; set; }

        public long? CreateBy { get; set; }

        public long? UpdateBy { get; set; }

        public bool? Deleted { get; set; }

        public ICollection<MasterMapColumn> Columns { get; set; } = new MasterMapColumn[0];
    }
}
