namespace TMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TMSMasterDataChanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("TMS.Pool", "PoolDescription", c => c.String(maxLength: 50));
            DropColumn("TMS.Pool", "PoolDescritpion");
        }
        
        public override void Down()
        {
            AddColumn("TMS.Pool", "PoolDescritpion", c => c.String(maxLength: 50));
            DropColumn("TMS.Pool", "PoolDescription");
        }
    }
}
