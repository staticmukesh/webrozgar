using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebRozgar.ViewModels
{
    public class RecruiterProfileViewModel
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

        [Display(Name="Company Name")]
        [Required(ErrorMessage = "Company name is required")]
        public string ComapnyName { get; set; }

        [Required(ErrorMessage = "Website is required ")]
        public string Website { get; set; }

        [Required(ErrorMessage = "Please select any one")]
        public string Type { get; set; }

        [Display(Name = "About Comapany")]
        [Required(ErrorMessage = "Company info is required")]
        public string AboutCompany { get; set; }

    }
}