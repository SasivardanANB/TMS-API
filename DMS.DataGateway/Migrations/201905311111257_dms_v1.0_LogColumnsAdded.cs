namespace DMS.DataGateway.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dms_v10_LogColumnsAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("DMS.PostalCode", "CreatedBy", c => c.String());
            AddColumn("DMS.PostalCode", "CreatedTime", c => c.DateTime());
            AddColumn("DMS.PostalCode", "LastModifiedBy", c => c.String());
            AddColumn("DMS.PostalCode", "LastModifiedTime", c => c.DateTime());
            AddColumn("DMS.SubDistrict", "CreatedBy", c => c.String());
            AddColumn("DMS.SubDistrict", "CreatedTime", c => c.DateTime());
            AddColumn("DMS.SubDistrict", "LastModifiedBy", c => c.String());
            AddColumn("DMS.SubDistrict", "LastModifiedTime", c => c.DateTime());
            AddColumn("DMS.Province", "CreatedBy", c => c.String());
            AddColumn("DMS.Province", "CreatedTime", c => c.DateTime());
            AddColumn("DMS.Province", "LastModifiedBy", c => c.String());
            AddColumn("DMS.Province", "LastModifiedTime", c => c.DateTime());
            AddColumn("DMS.ImageType", "CreatedBy", c => c.String());
            AddColumn("DMS.ImageType", "CreatedTime", c => c.DateTime());
            AddColumn("DMS.ImageType", "LastModifiedBy", c => c.String());
            AddColumn("DMS.ImageType", "LastModifiedTime", c => c.DateTime());
            AddColumn("DMS.PartnerPartnerType", "CreatedBy", c => c.String());
            AddColumn("DMS.PartnerPartnerType", "CreatedTime", c => c.DateTime());
            AddColumn("DMS.PartnerPartnerType", "LastModifiedBy", c => c.String());
            AddColumn("DMS.PartnerPartnerType", "LastModifiedTime", c => c.DateTime());
            AddColumn("DMS.ShipmentList", "CreatedBy", c => c.String());
            AddColumn("DMS.ShipmentList", "CreatedTime", c => c.DateTime());
            AddColumn("DMS.ShipmentList", "LastModifiedBy", c => c.String());
            AddColumn("DMS.ShipmentList", "LastModifiedTime", c => c.DateTime());
            AddColumn("DMS.TripStatus", "CreatedBy", c => c.String());
            AddColumn("DMS.TripStatus", "CreatedTime", c => c.DateTime());
            AddColumn("DMS.TripStatus", "LastModifiedBy", c => c.String());
            AddColumn("DMS.TripStatus", "LastModifiedTime", c => c.DateTime());
            AddColumn("DMS.StopPointImages", "CreatedBy", c => c.String());
            AddColumn("DMS.StopPointImages", "CreatedTime", c => c.DateTime());
            AddColumn("DMS.StopPointImages", "LastModifiedBy", c => c.String());
            AddColumn("DMS.StopPointImages", "LastModifiedTime", c => c.DateTime());
            AddColumn("DMS.TokenManager", "CreatedBy", c => c.String());
            AddColumn("DMS.TokenManager", "CreatedTime", c => c.DateTime());
            AddColumn("DMS.TokenManager", "LastModifiedBy", c => c.String());
            AddColumn("DMS.TokenManager", "LastModifiedTime", c => c.DateTime());
            AddColumn("DMS.TripStatusHistory", "CreatedBy", c => c.String());
            AddColumn("DMS.TripStatusHistory", "CreatedTime", c => c.DateTime());
            AddColumn("DMS.TripStatusHistory", "LastModifiedBy", c => c.String());
            AddColumn("DMS.TripStatusHistory", "LastModifiedTime", c => c.DateTime());
            AlterColumn("DMS.BusinessArea", "CreatedTime", c => c.DateTime());
            AlterColumn("DMS.City", "CreatedTime", c => c.DateTime());
            AlterColumn("DMS.ImageGuid", "CreatedBy", c => c.String());
            AlterColumn("DMS.ImageGuid", "CreatedTime", c => c.DateTime());
            AlterColumn("DMS.Partner", "CreatedTime", c => c.DateTime());
            AlterColumn("DMS.PartnerType", "CreatedTime", c => c.DateTime());
            AlterColumn("DMS.TripDetail", "CreatedBy", c => c.String());
            AlterColumn("DMS.TripDetail", "CreatedTime", c => c.DateTime());
            AlterColumn("DMS.TripHeader", "CreatedBy", c => c.String());
            AlterColumn("DMS.TripHeader", "CreatedTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("DMS.TripHeader", "CreatedTime", c => c.DateTime(nullable: false));
            AlterColumn("DMS.TripHeader", "CreatedBy", c => c.String());
            AlterColumn("DMS.TripDetail", "CreatedTime", c => c.DateTime(nullable: false));
            AlterColumn("DMS.TripDetail", "CreatedBy", c => c.String());
            AlterColumn("DMS.PartnerType", "CreatedTime", c => c.DateTime(nullable: false));
            AlterColumn("DMS.Partner", "CreatedTime", c => c.DateTime(nullable: false));
            AlterColumn("DMS.ImageGuid", "CreatedTime", c => c.DateTime(nullable: false));
            AlterColumn("DMS.ImageGuid", "CreatedBy", c => c.String());
            AlterColumn("DMS.City", "CreatedTime", c => c.DateTime(nullable: false));
            AlterColumn("DMS.BusinessArea", "CreatedTime", c => c.DateTime(nullable: false));
            DropColumn("DMS.TripStatusHistory", "LastModifiedTime");
            DropColumn("DMS.TripStatusHistory", "LastModifiedBy");
            DropColumn("DMS.TripStatusHistory", "CreatedTime");
            DropColumn("DMS.TripStatusHistory", "CreatedBy");
            DropColumn("DMS.TokenManager", "LastModifiedTime");
            DropColumn("DMS.TokenManager", "LastModifiedBy");
            DropColumn("DMS.TokenManager", "CreatedTime");
            DropColumn("DMS.TokenManager", "CreatedBy");
            DropColumn("DMS.StopPointImages", "LastModifiedTime");
            DropColumn("DMS.StopPointImages", "LastModifiedBy");
            DropColumn("DMS.StopPointImages", "CreatedTime");
            DropColumn("DMS.StopPointImages", "CreatedBy");
            DropColumn("DMS.TripStatus", "LastModifiedTime");
            DropColumn("DMS.TripStatus", "LastModifiedBy");
            DropColumn("DMS.TripStatus", "CreatedTime");
            DropColumn("DMS.TripStatus", "CreatedBy");
            DropColumn("DMS.ShipmentList", "LastModifiedTime");
            DropColumn("DMS.ShipmentList", "LastModifiedBy");
            DropColumn("DMS.ShipmentList", "CreatedTime");
            DropColumn("DMS.ShipmentList", "CreatedBy");
            DropColumn("DMS.PartnerPartnerType", "LastModifiedTime");
            DropColumn("DMS.PartnerPartnerType", "LastModifiedBy");
            DropColumn("DMS.PartnerPartnerType", "CreatedTime");
            DropColumn("DMS.PartnerPartnerType", "CreatedBy");
            DropColumn("DMS.ImageType", "LastModifiedTime");
            DropColumn("DMS.ImageType", "LastModifiedBy");
            DropColumn("DMS.ImageType", "CreatedTime");
            DropColumn("DMS.ImageType", "CreatedBy");
            DropColumn("DMS.Province", "LastModifiedTime");
            DropColumn("DMS.Province", "LastModifiedBy");
            DropColumn("DMS.Province", "CreatedTime");
            DropColumn("DMS.Province", "CreatedBy");
            DropColumn("DMS.SubDistrict", "LastModifiedTime");
            DropColumn("DMS.SubDistrict", "LastModifiedBy");
            DropColumn("DMS.SubDistrict", "CreatedTime");
            DropColumn("DMS.SubDistrict", "CreatedBy");
            DropColumn("DMS.PostalCode", "LastModifiedTime");
            DropColumn("DMS.PostalCode", "LastModifiedBy");
            DropColumn("DMS.PostalCode", "CreatedTime");
            DropColumn("DMS.PostalCode", "CreatedBy");
        }
    }
}
