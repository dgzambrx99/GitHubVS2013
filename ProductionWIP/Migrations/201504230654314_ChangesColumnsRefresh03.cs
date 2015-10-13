namespace ProductionWIP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangesColumnsRefresh03 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserHideColumns", "ColumnNamesId", c => c.Int(nullable: false));
            CreateIndex("dbo.UserHideColumns", "ColumnNamesId");
            AddForeignKey("dbo.UserHideColumns", "ColumnNamesId", "dbo.ColumnNames", "ColumnNamesId", cascadeDelete: true);
            DropColumn("dbo.UserHideColumns", "ColumnsId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserHideColumns", "ColumnsId", c => c.Int(nullable: false));
            DropForeignKey("dbo.UserHideColumns", "ColumnNamesId", "dbo.ColumnNames");
            DropIndex("dbo.UserHideColumns", new[] { "ColumnNamesId" });
            DropColumn("dbo.UserHideColumns", "ColumnNamesId");
        }
    }
}
