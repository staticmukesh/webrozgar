using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebRozgar.Models
{
    public class Job
    {
        public int JobId { get; set; }
        public string JobTitle { get; set; }
        public string Location { get; set; }
        public string NoOfOpenings { get; set; }
        public string Experience { get; set; }
        public string SkillsRequired { get; set; }
        public string Description { get; set; }
        public DateTime PostedDate { get; set; }
        
        public string JobFloaterUsername { get; set; }                           //Foreign Key
            
        public ICollection<UserProfile> JobAppliers { get; set; }       //List of application
    //
    }
}