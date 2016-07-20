using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebRozgar.ViewModels
{
    public class SearchResultViewModel
    {
        public int JobId { get; set; }
        public string JobTitle { get; set; }
        public string JobDescription { get; set; }
        public DateTime PostedDate { get; set; }
        public string Location { get; set; }
        public string FloaterUsername { get; set; }
    }
}