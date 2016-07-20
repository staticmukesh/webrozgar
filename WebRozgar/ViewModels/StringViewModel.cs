using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebRozgar.ViewModels
{
    public class StringViewModel
    {
        [Required]
        public string Data { get; set; }
    }
}