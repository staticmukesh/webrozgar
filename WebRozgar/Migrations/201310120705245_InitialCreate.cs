namespace WebRozgar.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Jobs",
                c => new
                    {
                        JobId = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Description = c.String(),
                        ExpiryDate = c.DateTime(nullable: false),
                        JobFloater = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.JobId);
            
            CreateTable(
                "dbo.Qualifications",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        Specification = c.String(),
                        ResumePath = c.String(),
                    })
                .PrimaryKey(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Qualifications");
            DropTable("dbo.Jobs");
        }
    }
}
