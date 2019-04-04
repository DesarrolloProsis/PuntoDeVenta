namespace PuntoDeVenta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BorrarExcepcion : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Historico", "Excepcion");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Historico", "Excepcion", c => c.String(maxLength: 30));
        }
    }
}
