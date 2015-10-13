namespace ProductionWIP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _2303201501 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Operators",
                c => new
                    {
                        //I will try to change this so the identity is created from the start
                        //OperatorsId = c.String(nullable: false, maxLength: 128),
                        OperatorsId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.OperatorsId);

            AddColumn("dbo.HeatTreat", "OperatorsId", c => c.Int());
            CreateIndex("dbo.HeatTreat", "OperatorsId");
            AddForeignKey("dbo.HeatTreat", "OperatorsId", "dbo.Operators", "OperatorsId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.HeatTreat", "OperatorsId", "dbo.Operators");
            DropIndex("dbo.HeatTreat", new[] { "OperatorsId" });
            DropColumn("dbo.HeatTreat", "OperatorsId");
            DropTable("dbo.Operators");
        }
    }
}
