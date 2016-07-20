using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebRozgar.Models
{
    public class JobApplication
    {
        [Key]
        public int AppId { get; set; }
        public int JobId { get; set; }
        public string UserName{ get; set; }
        public string status { get; set; }  //0 for shortlist , 1 for selected, -2 for not selected, -1 for not shortlisted
    }

}