namespace PuntoDeVenta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class requiredSaldoCuentaIsNull : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.CuentasTelepeajes", "SaldoCuenta", c => c.Double());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.CuentasTelepeajes", "SaldoCuenta", c => c.Double(nullable: false));
        }
    }
}
