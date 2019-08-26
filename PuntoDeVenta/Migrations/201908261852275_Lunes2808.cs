namespace PuntoDeVenta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Lunes2808 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Historico", "NumeroCuenta", c => c.String(maxLength: 20));
            AddColumn("dbo.Historico", "TipoCuenta", c => c.String(maxLength: 20));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Historico", "TipoCuenta");
            DropColumn("dbo.Historico", "NumeroCuenta");
        }
    }
}
