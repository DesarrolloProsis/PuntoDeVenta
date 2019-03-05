namespace PuntoDeVenta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CambioEnNombres : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Historico", name: "NumTag", newName: "Tag");
            RenameColumn(table: "dbo.Historico", name: "SaldoTag", newName: "Saldo");
        }
        
        public override void Down()
        {
            RenameColumn(table: "dbo.Historico", name: "Saldo", newName: "SaldoTag");
            RenameColumn(table: "dbo.Historico", name: "Tag", newName: "NumTag");
        }
    }
}
