namespace PuntoDeVenta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeopecajero : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.OperacionesCajeroes", "Tags_Id", "dbo.Tags");
            DropIndex("dbo.OperacionesCajeroes", new[] { "Tags_Id" });
            DropColumn("dbo.OperacionesCajeroes", "Tags_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OperacionesCajeroes", "Tags_Id", c => c.Long());
            CreateIndex("dbo.OperacionesCajeroes", "Tags_Id");
            AddForeignKey("dbo.OperacionesCajeroes", "Tags_Id", "dbo.Tags", "Id");
        }
    }
}
