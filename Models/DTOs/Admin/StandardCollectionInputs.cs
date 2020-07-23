using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using Newtonsoft.Json.Schema;
using System.ComponentModel.DataAnnotations;

namespace OriginsRx.Models.DTOs
{
    public enum Comparers
    {
        Equal = 1,
        NotEqual = 2,
        GreaterThan = 3,
        GreaterThanEqual = 4,
        LessThan = 5,
        LessThanEqual = 6,
        StartsWith = 7,
        Contains = 8
    }

    public class SearchParameter
    {
  //      public int OpenParenthesis { get; set; } = 0;
        [JsonRequired]
        public string Column { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Comparers Compare { get; set; }

        [JsonRequired]        
        public dynamic Value { get; set; }

        public bool Or { get; set; } = false;

//        public int CloseParenthesis { get; set; } = 0;
    }

    public class SortParameter
    {

        [JsonRequired]
        public string Column { get; set; }

        public bool Desc { get; set; } = false;
    }
    public class StdCollectionInputs : StdInputs
    {
        public int Offset { get; set; } = 0;

        public int Limit { get; set; } = 20;

        public bool IncludeDeleted { get; set; } = false;

       public ICollection<SortParameter> SortParameters { get; set; }

        [JsonIgnore]        
        public string Sort { get; set; } = "updateDT:desc";

    }

    public class StdCollectionInputsId : StdCollectionInputs
    {
        [JsonRequired]
        public long Id { get; set; }
    }

    public class StdInputs
    {
        public ICollection<SearchParameter> SearchParameters { get; set; }

        [JsonIgnore]
        public string Route { get; set; }

        [JsonIgnore]
        public long PersonId { get; set; }
    }
}
