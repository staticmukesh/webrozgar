namespace WebRozgar.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Web.Security;
    using WebMatrix.WebData;

    internal sealed class Configuration : DbMigrationsConfiguration<WebRozgar.DAL.WebRozgarContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(WebRozgar.DAL.WebRozgarContext context)
        {
            WebSecurity.InitializeDatabaseConnection("WebRozgarConnection", "UserProfile", "UserId", "UserName", autoCreateTables: true);
            var roles = (SimpleRoleProvider)Roles.Provider;

            if (!roles.RoleExists("admin"))
            {
                roles.CreateRole("admin");
            }
            if (!roles.RoleExists("recruiter"))
            {
                roles.CreateRole("recruiter");
            }
            if (!roles.RoleExists("seeker"))
            {
                roles.CreateRole("seeker");
            }

        }
    }
}
