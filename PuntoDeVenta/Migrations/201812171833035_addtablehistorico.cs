namespace PuntoDeVenta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addtablehistorico : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Historico",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NumTag = c.String(),
                        Carril = c.String(),
                        Fecha = c.DateTime(nullable: false),
                        SaldoTag = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Historico");
        }
    }
}
