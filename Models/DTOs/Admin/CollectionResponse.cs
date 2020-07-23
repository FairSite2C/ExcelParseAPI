using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OriginsRx.Models.DTOs
{
    public class CollectionResponse
    {
        public bool Success = true;

        public string Feedback;

        public List<string> Errors = new List<string>();

        public Dictionary<string, string> Info = new Dictionary<string, string>();

        public int TotalCount { get; set; }

        public int ItemCount { get; set; }

        public Link Paging { get; set; } = new Link();

    }

    public class Link
    {
        public string Self { get; set; }
        public string Prev { get; set; }
        public string Next { get; set; }
        public string First { get; set; }
        public string Last { get; set; }

    }
}
