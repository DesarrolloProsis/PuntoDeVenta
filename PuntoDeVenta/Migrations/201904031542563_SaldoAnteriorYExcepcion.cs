namespace PuntoDeVenta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SaldoAnteriorYExcepcion : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Historico", "SaldoAnterior", c => c.String(maxLength: 20));
            AddColumn("dbo.Historico", "Excepcion", c => c.String(maxLength: 30));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Historico", "Excepcion");
            DropColumn("dbo.Historico", "SaldoAnterior");
        }
    }
}
