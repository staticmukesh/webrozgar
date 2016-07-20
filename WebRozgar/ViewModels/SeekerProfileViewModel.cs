using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebRozgar.Helpers;

namespace WebRozgar.ViewModels
{
    public class SeekerProfileViewModel
    {

        public int IsProfileCompleted { get; set; }

        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Please select any one")]
        public string Gender { get; set; }

        [Required(ErrorMessage="Bio field is required")]
        [Display(Name="Bio/About you")]
        public string Bio { get; set; }
        
        [Required(ErrorMessage="Please select any one")]
        public string Education { get; set; }

        [Required(ErrorMessage = "Enter the work experience")]
        public string WorkExperience { get; set; }

        [Required(ErrorMessage = "Enter the work experience")]
        public string University { get; set; }
        


        [Required(ErrorMessage="Resume is required")]
        [MaxSize(2097152)]
        public HttpPostedFileBase Resume { get; set; }

    }
}