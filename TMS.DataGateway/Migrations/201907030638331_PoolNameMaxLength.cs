namespace TMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PoolNameMaxLength : DbMigration
    {
        public override void Up()
        {
            DropIndex("TMS.Pool", "Pool_PoolName");
            AlterColumn("TMS.Pool", "PoolName", c => c.String(maxLength: 25));
            CreateIndex("TMS.Pool", "PoolName", unique: true, name: "Pool_PoolName");
        }
        
        public override void Down()
        {
            DropIndex("TMS.Pool", "Pool_PoolName");
            AlterColumn("TMS.Pool", "PoolName", c => c.String(maxLength: 10));
            CreateIndex("TMS.Pool", "PoolName", unique: true, name: "Pool_PoolName");
        }
    }
}
