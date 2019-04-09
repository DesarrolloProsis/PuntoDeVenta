namespace PuntoDeVenta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NuevaColumna : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Historico", "SaldoActualizado", c => c.String(maxLength: 20));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Historico", "SaldoActualizado");
        }
    }
}
