namespace PuntoDeVenta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class modificamosHistorico : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Historico", "Tag", c => c.String());
            AddColumn("dbo.Historico", "Delegacion", c => c.String());
            AddColumn("dbo.Historico", "Plaza", c => c.String());
            AddColumn("dbo.Historico", "Cuerpo", c => c.String());
            AddColumn("dbo.Historico", "Clase", c => c.String());
            AddColumn("dbo.Historico", "Saldo", c => c.String());
            AddColumn("dbo.Historico", "Operador", c => c.String());
            AlterColumn("dbo.Historico", "Fecha", c => c.String());
            DropColumn("dbo.Historico", "NumTag");
            DropColumn("dbo.Historico", "SaldoTag");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Historico", "SaldoTag", c => c.Single(nullable: false));
            AddColumn("dbo.Historico", "NumTag", c => c.String());
            AlterColumn("dbo.Historico", "Fecha", c => c.DateTime(nullable: false));
            DropColumn("dbo.Historico", "Operador");
            DropColumn("dbo.Historico", "Saldo");
            DropColumn("dbo.Historico", "Clase");
            DropColumn("dbo.Historico", "Cuerpo");
            DropColumn("dbo.Historico", "Plaza");
            DropColumn("dbo.Historico", "Delegacion");
            DropColumn("dbo.Historico", "Tag");
        }
    }
}
