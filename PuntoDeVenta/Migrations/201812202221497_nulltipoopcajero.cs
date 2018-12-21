namespace PuntoDeVenta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nulltipoopcajero : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.OperacionesCajeroes", "TipoPago", c => c.String(maxLength: 30));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.OperacionesCajeroes", "TipoPago", c => c.String(nullable: false, maxLength: 30));
        }
    }
}
