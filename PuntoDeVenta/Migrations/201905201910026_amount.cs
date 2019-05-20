namespace PuntoDeVenta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class amount : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AmountConfigurations",
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
            DropTable("dbo.AmountConfigurations");
        }
    }
}
