namespace OMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserEmailPhoneNumber : DbMigration
    {
        public override void Up()
        {
            AddColumn("OMS.User", "Email", c => c.String(nullable: false, maxLength: 50));
            AddColumn("OMS.User", "PhoneNumber", c => c.String(maxLength: 15));
        }
        
        public override void Down()
        {
            DropColumn("OMS.User", "PhoneNumber");
            DropColumn("OMS.User", "Email");
        }
    }
}
