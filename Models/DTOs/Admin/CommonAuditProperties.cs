using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using System.ComponentModel.DataAnnotations;

namespace OriginsRx.Models.DTOs
{
    /// <summary>
    /// Common audit properties included for entities
    /// </summary>
    public class CommonAuditProperties 
    {
 
        public DateTime? CreateDate { get; set; }

        public string CreateBy { get; set; }

        public DateTime? UpdateDate { get; set; }

        [MaxLength(32)]
        public string UpdateBy { get; set; } 

        public bool Deleted { get; set; } = false;

    }
}
