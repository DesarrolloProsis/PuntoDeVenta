namespace PuntoDeVenta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class saldotagnull : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Tags", "SaldoTag", c => c.Double());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Tags", "SaldoTag", c => c.Double(nullable: false));
        }
    }
}
