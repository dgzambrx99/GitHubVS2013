namespace ProductionWIP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _2015032502 : DbMigration
    {
        public override void Up()
        {

            AddColumn("dbo.HeatTreat", "Image2", c => c.Boolean(nullable: false));
            AddColumn("dbo.HeatTreat", "Image3", c => c.Boolean(nullable: false));
            AddColumn("dbo.HeatTreat", "Image4", c => c.Boolean(nullable: false));
            AddColumn("dbo.HeatTreat", "Image5", c => c.Boolean(nullable: false));
            AlterColumn("dbo.HeatTreat", "Image", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.HeatTreat", "Image", c => c.String());
            DropColumn("dbo.HeatTreat", "Image5");
            DropColumn("dbo.HeatTreat", "Image4");
            DropColumn("dbo.HeatTreat", "Image3");
            DropColumn("dbo.HeatTreat", "Image2");
        }
    }
}
