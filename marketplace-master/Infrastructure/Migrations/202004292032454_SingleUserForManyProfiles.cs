namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class SingleUserForManyProfiles : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Person", "UserId", c => c.String(maxLength: 128, unicode: false));
            
            Sql(@"
                UPDATE dbo.Person SET UserId = dbo.AspNetUsers.Id
                FROM dbo.Person
                JOIN dbo.AspNetUsers ON dbo.Person.Id = dbo.AspNetUsers.ProfileId");

            AlterColumn("dbo.Person", "UserId", c => c.String(nullable: false, maxLength: 128, unicode: false));
            AddForeignKey("dbo.Person", "UserId", "dbo.AspNetUsers", cascadeDelete: true);
            CreateIndex("dbo.Person", new[] { "UserId" });

            DropColumn("dbo.AspNetUsers", "ProfileId");
            DropColumn("dbo.AspNetUsers", "ProfileType");
        }

        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "ProfileType", c => c.String(maxLength: 30, unicode: false));
            AddColumn("dbo.AspNetUsers", "ProfileId", c => c.String(maxLength: 128, unicode: false));

            Sql(@"
                UPDATE dbo.AspNetUsers SET ProfileId = Person.Id, ProfileType = Person.Discriminator
                FROM AspNetUsers
                JOIN Person ON AspNetUsers.Id = Person.UserId");

            DropIndex("dbo.Person", new[] { "UserId " });
            DropForeignKey("dbo.Person", "UserId", "dbo.AspNetUsers");
            DropColumn("dbo.Person", "UserId");
        }
    }
}
