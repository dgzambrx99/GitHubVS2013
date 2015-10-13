namespace ProductionWIP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _2015031401 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.HeatTreat", "Load", c => c.String(nullable: false));
            AddColumn("dbo.HeatTreat", "Image", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.HeatTreat", "Image");
            DropColumn("dbo.HeatTreat", "Load");
        }
    }
}
