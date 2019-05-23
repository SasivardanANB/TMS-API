namespace OMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserNamechanges : DbMigration
    {
        public override void Up()
        {
            DropIndex("OMS.User", "User_UserName");
        }
        
        public override void Down()
        {
            CreateIndex("OMS.User", "UserName", unique: true, name: "User_UserName");
        }
    }
}
