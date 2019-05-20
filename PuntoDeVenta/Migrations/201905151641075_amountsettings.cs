namespace PuntoDeVenta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class amountsettings : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AmountSettings",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Concept = c.String(maxLength: 20),
                        Type = c.Short(nullable: false),
                        Amount = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.AmountSettings");
        }
    }
}
