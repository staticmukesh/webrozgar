using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebRozgar.Models
{
    public class CompanyInfo
    {
        [Key]
        public string UserName { get; set; }
        public string ComapnyName { get; set; }
        public string Website { get; set; }
        public string Type { get; set; }
        public string AboutComapany { get; set; }
    }
}