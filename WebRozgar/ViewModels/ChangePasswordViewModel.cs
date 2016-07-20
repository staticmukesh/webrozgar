using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebRozgar.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage="Current Password is required")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage="Password is required")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage="Confirm Password is required")]
        [Compare("NewPassword",ErrorMessage="Password does not match")]
        public string ConfirmNewPassword { get; set; }
    }
}