namespace PuntoDeVenta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changebdlistanegra : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.OperacionesCajeroes", "TagId", "dbo.Tags");
            DropForeignKey("dbo.OperacionesCajeroes", "CortesCajero_Id", "dbo.CortesCajeroes");
            DropIndex("dbo.OperacionesCajeroes", new[] { "TagId" });
            DropIndex("dbo.OperacionesCajeroes", new[] { "CortesCajero_Id" });
            DropColumn("dbo.OperacionesCajeroes", "CorteId");
            RenameColumn(table: "dbo.OperacionesCajeroes", name: "TagId", newName: "Tags_Id");
            RenameColumn(table: "dbo.OperacionesCajeroes", name: "CortesCajero_Id", newName: "CorteId");
            CreateTable(
                "dbo.ListaNegra",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Tipo = c.String(nullable: false, maxLength: 30),
                        Numero = c.String(nullable: false, maxLength: 50),
                        Observacion = c.String(nullable: false, maxLength: 280),
                        SaldoAnterior = c.Double(),
                        Date = c.DateTime(nullable: false),
                        IdCajero = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.OperacionesCajeroes", "Numero", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.OperacionesCajeroes", "Monto", c => c.Double());
            AlterColumn("dbo.OperacionesCajeroes", "Tags_Id", c => c.Long());
            AlterColumn("dbo.OperacionesCajeroes", "CorteId", c => c.Long(nullable: false));
            CreateIndex("dbo.OperacionesCajeroes", "CorteId");
            CreateIndex("dbo.OperacionesCajeroes", "Tags_Id");
            AddForeignKey("dbo.OperacionesCajeroes", "Tags_Id", "dbo.Tags", "Id");
            AddForeignKey("dbo.OperacionesCajeroes", "CorteId", "dbo.CortesCajeroes", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OperacionesCajeroes", "CorteId", "dbo.CortesCajeroes");
            DropForeignKey("dbo.OperacionesCajeroes", "Tags_Id", "dbo.Tags");
            DropIndex("dbo.OperacionesCajeroes", new[] { "Tags_Id" });
            DropIndex("dbo.OperacionesCajeroes", new[] { "CorteId" });
            AlterColumn("dbo.OperacionesCajeroes", "CorteId", c => c.Long());
            AlterColumn("dbo.OperacionesCajeroes", "Tags_Id", c => c.Long(nullable: false));
            AlterColumn("dbo.OperacionesCajeroes", "Monto", c => c.Double(nullable: false));
            DropColumn("dbo.OperacionesCajeroes", "Numero");
            DropTable("dbo.ListaNegra");
            RenameColumn(table: "dbo.OperacionesCajeroes", name: "CorteId", newName: "CortesCajero_Id");
            RenameColumn(table: "dbo.OperacionesCajeroes", name: "Tags_Id", newName: "TagId");
            AddColumn("dbo.OperacionesCajeroes", "CorteId", c => c.Long(nullable: false));
            CreateIndex("dbo.OperacionesCajeroes", "CortesCajero_Id");
            CreateIndex("dbo.OperacionesCajeroes", "TagId");
            AddForeignKey("dbo.OperacionesCajeroes", "CortesCajero_Id", "dbo.CortesCajeroes", "Id");
            AddForeignKey("dbo.OperacionesCajeroes", "TagId", "dbo.Tags", "Id", cascadeDelete: true);
        }
    }
}
