namespace PuntoDeVenta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class emihisto : DbMigration
    {
        public override void Up()
        {
            //AddColumn("dbo.Historico", "Evento", c => c.String(maxLength: 11));
            AddColumn("dbo.Historico", "SaldoAnterior", c => c.String(maxLength: 20));
            AddColumn("dbo.Historico", "SaldoActualizado", c => c.String(maxLength: 20));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Historico", "SaldoActualizado");
            DropColumn("dbo.Historico", "SaldoAnterior");
            //DropColumn("dbo.Historico", "Evento");
        }
    }
}
