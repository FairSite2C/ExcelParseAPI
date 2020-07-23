using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using System.ComponentModel.DataAnnotations;

namespace OriginsRx.Models.DTOs
{
    public class Company
    {
        public long Id { get; set; }

        [JsonRequired]
        public string Name { get; set; }
   
        public DateTime? CreateDT { get; set; }

        public DateTime? UpdateDT { get; set; }

        public long? CreateBy { get; set; }

        public long? UpdateBy { get; set; }

        public bool? Deleted { get; set; }
    }

    public class CompanyAdd
    {
        [JsonRequired]
        public string Name { get; set; }
    }

    public class CompanyMod
    {
        [JsonRequired]
        public long Id { get; set; }

        [JsonRequired]
        public string Name { get; set; }
    }
 }
