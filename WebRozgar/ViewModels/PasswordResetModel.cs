using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebRozgar.ViewModels
{
    public class PasswordResetModel
    {
        [Required]
        public string ResetToken { get; set; }

        [Required(ErrorMessage="Password is required")]
        public string Password { get; set; }
        
        [Required(ErrorMessage="Confirm Password is required")]
        [Compare("Password",ErrorMessage="Password does not match")]
        public string ConfirmPassword { get; set; }

    }
}