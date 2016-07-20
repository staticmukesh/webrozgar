using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebRozgar.ViewModels
{
    public class ProfileViewModel
    {

        [Required(ErrorMessage="First name is required")]
        [Display(Name="First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage="Last name is required")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required(ErrorMessage="Phone number is required")]
        [DataType(DataType.PhoneNumber,ErrorMessage="Phone number is not valid")]
        public string Phone { get; set; }

        public int IsProfileCompleted { get; set; }
    }
}