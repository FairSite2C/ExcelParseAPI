using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using System.ComponentModel.DataAnnotations;

namespace OriginsRx.Models.DTOs
{
    public class MasterMapColumn
    {
        public long? MasterMapID { get; set; }

        public string Header { get; set; }

        public string ColumnName { get; set; }

        public int ColumnDataType { get; set; }

        public bool Required { get; set; }

        public bool MakeIndex { get; set; }

        public long Id { get; set; }

        public DateTime? CreateDT { get; set; }

        public DateTime? UpdateDT { get; set; }

        public long? CreateBy { get; set; }

        public long? UpdateBy { get; set; }

        public bool? Deleted { get; set; }

    }
}
