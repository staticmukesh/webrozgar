using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebRozgar.ViewModels
{
    public class FloatJobViewModel
    {
        [Required(ErrorMessage="Job title is required")]
        [Display(Name="Job Title")]
        public string JobTitle { get; set; }
        
        [Required(ErrorMessage = "Job location is required")]
        public string Location { get; set; }

        [Required(ErrorMessage = "No of openings is required")]
        [Display(Name = "No of Openings")]
        public string NoOfOpenings { get; set; }

        [Required(ErrorMessage = "Experience is required")]
        [Display(Name = "Work Experience")]
        public string Experience { get; set; }

        [Required(ErrorMessage = "Skills are required")]
        [Display(Name = "Skills Required")]
        public string SkillsRequired { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        public DateTime PostedDate { get; set; }
        public int JobId { get; set; }
        public string FloatUsername { get; set; }
        public string Status { get; set; }
    }
}