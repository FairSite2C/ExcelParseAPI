using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using System.ComponentModel.DataAnnotations;

namespace OriginsRx.Models.DTOs
{
    public class Error
    {
        
        [Required]
        [StringLength(100)]
        public string Title { get; set;}

        [Required]
        [StringLength(400)]
        public string Detail { get;set; }

        [Range(400, 600)]
        public int Status { get; set;}
        // minimum: 100 maximum: 600

        [StringLength(100)]
        [Display(Description = "Example Test")]
        public string Type { get; set; }
        
        [StringLength(100)]
        public string Instance { get; set; }

    }

}
