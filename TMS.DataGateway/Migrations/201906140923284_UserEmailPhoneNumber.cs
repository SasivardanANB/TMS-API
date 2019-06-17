namespace TMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserEmailPhoneNumber : DbMigration
    {
        public override void Up()
        {
            AddColumn("TMS.User", "Email", c => c.String(nullable: false, maxLength: 50));
            AddColumn("TMS.User", "PhoneNumber", c => c.String(maxLength: 15));
        }
        
        public override void Down()
        {
            DropColumn("TMS.User", "PhoneNumber");
            DropColumn("TMS.User", "Email");
        }
    }
}
