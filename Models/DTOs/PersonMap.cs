using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using System.ComponentModel.DataAnnotations;

namespace OriginsRx.Models.DTOs
{
    public class PersonMap
    {
        public long Id { get; set; }

        [JsonRequired]
        public string Name { get; set; }

        [JsonRequired]
        public long PersonId { get; set; }

        [JsonRequired]
        public long MasterMapId { get; set; }

        public bool? Locked { get; set; }

        public DateTime? CreateDT { get; set; }

        public DateTime? UpdateDT { get; set; }

        public long? CreateBy { get; set; }

        public long? UpdateBy { get; set; }

        public bool? Deleted { get; set; }
 
        public ICollection<PersonMapColumn> Columns { get; set; }

    }

    public class PersonMapAdd
    {
        [JsonRequired]
        public string Name { get; set; }

        [JsonRequired]
        public long PersonId { get; set; }

        [JsonRequired]
        public long MasterMapId { get; set; }

        [MinLength(1)]
        public ICollection<PersonMapColumnAdd> Columns { get; set; }

    }

    public class PersonMapMod
    {
        [JsonRequired]
        public long Id { get; set; }

        public string Name { get; set; }

        public long PersonId { get; set; }

        public ICollection<PersonMapColumnMod> Updates { get; set; }

        public List<int> Deletes { get; set; }

        public ICollection<PersonMapColumnAdd> Adds { get; set; }
    }
}
