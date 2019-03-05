namespace PuntoDeVenta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NombresColumn : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Historico","NumTag");
            DropColumn("dbo.Historico", "SaldoTag");
            RenameColumn("dbo.Historico", "NumTag", "Tag");
            RenameColumn("dbo.Historico", "SaldoTag", "Saldo");
        }
        
        public override void Down()
        {
            DropColumn("dbo.Historico", "Tag");
            DropColumn("dbo.Historico", "Saldo");
            RenameColumn("dbo.Historico", "Tag", "NumTag");
            RenameColumn("dbo.Historico", "Saldo", "SaldoTag");
        }
    }
}
