namespace PuntoDeVenta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class chagesaldosvarchar : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.CuentasTelepeajes", "SaldoCuenta", c => c.String(maxLength: 20));
            AlterColumn("dbo.Tags", "SaldoTag", c => c.String(maxLength: 20));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Tags", "SaldoTag", c => c.Double());
            AlterColumn("dbo.CuentasTelepeajes", "SaldoCuenta", c => c.Double());
        }
    }
}
