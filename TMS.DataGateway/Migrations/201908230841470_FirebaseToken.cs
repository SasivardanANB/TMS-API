namespace TMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FirebaseToken : DbMigration
    {
        public override void Up()
        {
            AddColumn("TMS.Token", "FirebaseToken", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("TMS.Token", "FirebaseToken");
        }
    }
}
