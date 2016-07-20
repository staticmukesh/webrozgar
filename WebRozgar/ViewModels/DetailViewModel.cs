using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebRozgar.Models;

namespace WebRozgar.ViewModels
{
    public class DetailViewModel
    {
        public FloatJobViewModel jobmodel { get; set; }
        public IQueryable<JobApplicationViewModel> applicationmodel { get; set; }
    }
}