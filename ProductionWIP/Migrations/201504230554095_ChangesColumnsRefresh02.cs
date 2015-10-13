namespace ProductionWIP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangesColumnsRefresh02 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RTs",
                c => new
                    {
                        RTId = c.Int(nullable: false, identity: true),
                        LastModified = c.DateTime(nullable: false),
                        OperatorsId = c.Int(),
                        AdminId = c.String(maxLength: 128),
                        bitDevice = c.Boolean(),
                    })
                .PrimaryKey(t => t.RTId)
                .ForeignKey("dbo.AspNetUsers", t => t.AdminId)
                .ForeignKey("dbo.Operators", t => t.OperatorsId)
                .Index(t => t.OperatorsId)
                .Index(t => t.AdminId);
            
            CreateTable(
                "dbo.UserHideColumns",
                c => new
                    {
                        UserHideColumnsId = c.Int(nullable: false, identity: true),
                        OperatorsId = c.Int(nullable: false),
                        ColumnsId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserHideColumnsId)
                .ForeignKey("dbo.Operators", t => t.OperatorsId, cascadeDelete: true)
                .Index(t => t.OperatorsId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserHideColumns", "OperatorsId", "dbo.Operators");
            DropForeignKey("dbo.RTs", "OperatorsId", "dbo.Operators");
            DropForeignKey("dbo.RTs", "AdminId", "dbo.AspNetUsers");
            DropIndex("dbo.UserHideColumns", new[] { "OperatorsId" });
            DropIndex("dbo.RTs", new[] { "AdminId" });
            DropIndex("dbo.RTs", new[] { "OperatorsId" });
            DropTable("dbo.UserHideColumns");
            DropTable("dbo.RTs");
        }
    }
}
