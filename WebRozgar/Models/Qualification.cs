using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebRozgar.Models
{
    public class Qualification
    {
        [Key]
        public string UserName { get; set; }
        public string Bio { get; set; }
        public string Education { get; set; }
        public string WorkExperience { get; set; }
        public string University { get; set; }
        public string ResumePath { get; set; }
    }
}