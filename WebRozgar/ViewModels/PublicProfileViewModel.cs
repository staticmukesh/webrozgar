using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebRozgar.ViewModels
{
    public class PublicProfileViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Gender { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }

        public string Bio { get; set; }
        public string Education { get; set; }
        public string WorkExperience { get; set; }
        public string University { get; set; }
        public string Resume { get; set; }

        public string ComapnyName { get; set; }
        public string Website { get; set; }
        public string Type { get; set; }
        public string AboutCompany { get; set; }
    }
}