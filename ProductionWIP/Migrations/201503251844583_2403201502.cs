namespace ProductionWIP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _2403201502 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ColumnNames",
                c => new
                    {
                        ColumnNamesId = c.Int(nullable: false, identity: true),
                        Order = c.Int(nullable: false),
                        Label = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ColumnNamesId);
            
            CreateTable(
                "dbo.EmbedCodes",
                c => new
                    {
                        EmbedCodesId = c.Int(nullable: false, identity: true),
                        Identifier = c.String(nullable: false),
                        Token = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.EmbedCodesId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.EmbedCodes");
            DropTable("dbo.ColumnNames");
        }
    }
}
