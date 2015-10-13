namespace ProductionWIP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _2015040101 : DbMigration
    {
        public override void Up()
        {
            //Here you see the changes that will be applied, in this case an alter column
            //When things are more complex, you can use sql statements to alter the data like
            //Sql("UPDATE HeatTreat SET Jobcardnumber='n' where 1=2");
            AlterColumn("dbo.HeatTreat", "JobCardNumber", c => c.String(nullable: false, maxLength: 50));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.HeatTreat", "JobCardNumber", c => c.String(nullable: false));
        }
    }
}
