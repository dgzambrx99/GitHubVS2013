namespace ProductionWIP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _2015032501 : DbMigration
    {
        public override void Up()
        {
            //DropForeignKey("dbo.HeatTreat", "OperatorsId", "dbo.Operators");
            //DropIndex("dbo.HeatTreat", new[] { "OperatorsId" });
            //DropPrimaryKey("dbo.Operators");
            AddColumn("dbo.Operators", "Number", c => c.String(nullable: false));
            //AlterColumn("dbo.HeatTreat", "OperatorsId", c => c.Int());
            //AlterColumn("dbo.Operators", "OperatorsId", c => c.Int(nullable: false, identity: true));
            //AddPrimaryKey("dbo.Operators", "OperatorsId");
            //CreateIndex("dbo.HeatTreat", "OperatorsId");
            //AddForeignKey("dbo.HeatTreat", "OperatorsId", "dbo.Operators", "OperatorsId");
        }
        
        public override void Down()
        {
            //DropForeignKey("dbo.HeatTreat", "OperatorsId", "dbo.Operators");
            //DropIndex("dbo.HeatTreat", new[] { "OperatorsId" });
            //DropPrimaryKey("dbo.Operators");
            //AlterColumn("dbo.Operators", "OperatorsId", c => c.String(nullable: false, maxLength: 128));
            //AlterColumn("dbo.HeatTreat", "OperatorsId", c => c.String(maxLength: 128));
            DropColumn("dbo.Operators", "Number");
            //AddPrimaryKey("dbo.Operators", "OperatorsId");
            //CreateIndex("dbo.HeatTreat", "OperatorsId");
            //AddForeignKey("dbo.HeatTreat", "OperatorsId", "dbo.Operators", "OperatorsId");
        }
    }
}
