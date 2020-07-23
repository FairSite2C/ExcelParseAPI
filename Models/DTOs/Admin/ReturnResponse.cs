using System;
using System.Collections.Generic;
using System.Text;

namespace OriginsRx.Models.DTOs
{
    public class  ReturnResponse<T>
    { 
        public bool Success {
            get { return Errors.Count == 0; }
        }

        public string Feedback;

        public List<string> Errors = new List<string>();

        public Dictionary<string, string> Info = new Dictionary<string, string>();

        public T Result;
    }
}
