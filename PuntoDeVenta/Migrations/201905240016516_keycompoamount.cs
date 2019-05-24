namespace PuntoDeVenta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class keycompoamount : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.AmountConfigurations");
            AlterColumn("dbo.AmountConfigurations", "Concept", c => c.String(nullable: false, maxLength: 20));
            AddPrimaryKey("dbo.AmountConfigurations", new[] { "Concept", "Amount" });
            DropColumn("dbo.AmountConfigurations", "Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AmountConfigurations", "Id", c => c.Long(nullable: false, identity: true));
            DropPrimaryKey("dbo.AmountConfigurations");
            AlterColumn("dbo.AmountConfigurations", "Concept", c => c.String(maxLength: 20));
            AddPrimaryKey("dbo.AmountConfigurations", "Id");
        }
    }
}
