using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using System.ComponentModel.DataAnnotations;

namespace OriginsRx.Models.DTOs
{
    public class PersonMapColumn
    {
        [JsonRequired]
        public long? PersonMapID { get; set; }

        [JsonRequired]
        public string OurHeader { get; set; }

        [JsonRequired]
        public string TheirHeader { get; set; }

        public long Id { get; set; }

        public DateTime? CreateDT { get; set; }

        public DateTime? UpdateDT { get; set; }

        public long? CreateBy { get; set; }

        public long? UpdateBy { get; set; }

        public bool? Deleted { get; set; }

    }

    public class PersonMapColumnAdd
    {
        [JsonRequired]
        public string OurHeader { get; set; }

        [JsonRequired]
        public string TheirHeader { get; set; }

        public string Regex { get; set; }

    }

    public class PersonMapColumnMod
    {
        public long id { get; set; }

        public string TheirHeader { get; set; }

        public string Regex { get; set; }
    }
}
