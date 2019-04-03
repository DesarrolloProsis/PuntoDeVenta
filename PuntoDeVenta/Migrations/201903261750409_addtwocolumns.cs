namespace PuntoDeVenta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addtwocolumns : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OperacionesCajeroes", "NoReferencia", c => c.String());
            AddColumn("dbo.OperacionesCajeroes", "StatusCancelacion", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OperacionesCajeroes", "StatusCancelacion");
            DropColumn("dbo.OperacionesCajeroes", "NoReferencia");
        }
    }
}
