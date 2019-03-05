namespace PuntoDeVenta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NuTagXTag : DbMigration
    {
        public override void Up()
        {
            RenameColumn("dbo.Historico", "NumTag", "Tag");
            RenameColumn("dbo.Historico", "SaldoTag", "Saldo");
        }
        
        public override void Down()
        {
            RenameColumn("dbo.Historico", "Tag", "NumTag");
            RenameColumn("dbo.Historico", "Saldo", "SaldoTag");
        }
    }
}
