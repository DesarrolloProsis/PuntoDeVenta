namespace PuntoDeVenta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CambioDeTamaños : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Historico", "NumTag", c => c.String(maxLength: 25));
            AddColumn("dbo.Historico", "SaldoTag", c => c.Double(nullable: false));
            AlterColumn("dbo.Historico", "Delegacion", c => c.String(maxLength: 35));
            AlterColumn("dbo.Historico", "Plaza", c => c.String(maxLength: 35));
            AlterColumn("dbo.Historico", "Fecha", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Historico", "Cuerpo", c => c.String(maxLength: 10));
            AlterColumn("dbo.Historico", "Carril", c => c.String(maxLength: 10));
            AlterColumn("dbo.Historico", "Clase", c => c.String(maxLength: 10));
            AlterColumn("dbo.Historico", "Operador", c => c.String(maxLength: 20));
            DropColumn("dbo.Historico", "Tag");
            DropColumn("dbo.Historico", "Saldo");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Historico", "Saldo", c => c.String());
            AddColumn("dbo.Historico", "Tag", c => c.String());
            AlterColumn("dbo.Historico", "Operador", c => c.String());
            AlterColumn("dbo.Historico", "Clase", c => c.String());
            AlterColumn("dbo.Historico", "Carril", c => c.String());
            AlterColumn("dbo.Historico", "Cuerpo", c => c.String());
            AlterColumn("dbo.Historico", "Fecha", c => c.String());
            AlterColumn("dbo.Historico", "Plaza", c => c.String());
            AlterColumn("dbo.Historico", "Delegacion", c => c.String());
            DropColumn("dbo.Historico", "SaldoTag");
            DropColumn("dbo.Historico", "NumTag");
        }
    }
}
