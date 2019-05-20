namespace PuntoDeVenta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class cambiosolivier : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.HistoricoListas",
                c => new
                {
                    Id = c.Long(nullable: false, identity: true),
                    Extension = c.String(),
                    Tamaño = c.String(),
                    Fecha_Creacion = c.DateTime(nullable: false),
                    Tipo = c.String(),
                })
                .PrimaryKey(t => t.Id);

            AddColumn("dbo.OperacionesSerBIpagos", "Numero", c => c.String(maxLength: 30));
            AddColumn("dbo.OperacionesSerBIpagos", "NoReferencia", c => c.String(maxLength: 30));
            AddColumn("dbo.OperacionesSerBIpagos", "Tipo", c => c.String(maxLength: 20));
            AddColumn("dbo.OperacionesSerBIpagos", "Concepto", c => c.String(maxLength: 30));
        }

        public override void Down()
        {
            DropColumn("dbo.OperacionesSerBIpagos", "Concepto");
            DropColumn("dbo.OperacionesSerBIpagos", "Tipo");
            DropColumn("dbo.OperacionesSerBIpagos", "NoReferencia");
            DropColumn("dbo.OperacionesSerBIpagos", "Numero");
            DropTable("dbo.HistoricoListas");
        }
    }
}
