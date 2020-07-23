using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using System.ComponentModel.DataAnnotations;

namespace OriginsRx.Models.DTOs
{
    public class Person
    {
        public long Id { get; set; }

        [JsonRequired]
        public string Name { get; set; }

        [JsonRequired]
        public long? CompanyId { get; set; }

        [JsonRequired]
        public string Email { get; set; }

        public DateTime? CreateDT { get; set; }

        public DateTime? UpdateDT { get; set; }

        public long? CreateBy { get; set; }

        public long? UpdateBy { get; set; }

        public bool? Deleted { get; set; }

    }

    public class PersonAdd
    {
    
        [JsonRequired]
        public string Name { get; set; }

        [JsonRequired]
        public long? CompanyId { get; set; }

        [JsonRequired]
        public string Email { get; set; }

    }

    public class PersonMod
    {
        [JsonRequired]
        public long Id { get; set; }

        public string Name { get; set; }

        public long? CompanyId { get; set; }

        public string Email { get; set; }

    }
}
