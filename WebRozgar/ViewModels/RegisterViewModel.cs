using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebRozgar.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Username is Required")]
        [Display(Name = "User name")]
        [Remote("CheckUsername", "Account", ErrorMessage = "Username is not available")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is Required")]
        [Display(Name = "Email")]
        [RegularExpression(".+\\@.+\\..+", ErrorMessage = "Please enter valid email address")]
        [Remote("CheckEmail", "Account", ErrorMessage = "Email is not available")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is Required")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirmed Password is Required")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.Web.Mvc.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string TypeOfUser { get; set; }

    }
}