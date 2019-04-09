namespace PuntoDeVenta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class emx : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Historico", "Evento", c => c.String(maxLength: 10));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Historico", "Evento", c => c.String(maxLength: 11));
        }
    }
}
