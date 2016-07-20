using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using WebMatrix.WebData;
using WebRozgar.Models;

namespace WebRozgar.DAL
{
    public class WebRozgarContext : DbContext
    {
        public WebRozgarContext() : base("WebRozgarConnection") { }

        public DbSet<Job> Jobs { get; set; }
        public DbSet<Qualification> Qualifications { get; set; }
        public DbSet<CompanyInfo> CompanyInfos { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<JobApplication> JobApplications { get; set; }
        public DbSet<Message> Messages { get; set; }

    }

}