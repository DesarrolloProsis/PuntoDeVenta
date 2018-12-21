namespace PuntoDeVenta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addtipoopcajero : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OperacionesCajeroes", "Tipo", c => c.String(nullable: false, maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OperacionesCajeroes", "Tipo");
        }
    }
}
