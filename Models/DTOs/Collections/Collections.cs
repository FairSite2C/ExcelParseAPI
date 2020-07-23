using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OriginsRx.Models.DTOs
{
    public class Companies : CollectionResponse
    {
        public IEnumerable<Company> Items { get; set; }
    }

    public class Persons : CollectionResponse
    {
        public IEnumerable<Person> Items { get; set; }
    }

    public class MasterMaps : CollectionResponse
    {
        public IEnumerable<MasterMap> Items { get; set; }
    }

    public class PersonMaps : CollectionResponse
    {
        public IEnumerable<PersonMap> Items { get; set; }
    }

    public class Imports : CollectionResponse
    {
        public IEnumerable<Import> Items { get; set; }
    }

    public class Sales : CollectionResponse
    {
        public IEnumerable<Sale> Items { get; set; }
    }
}
