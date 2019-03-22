namespace PuntoDeVenta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class phoneno : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Clientes", "PhoneCliente", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Clientes", "PhoneCliente", c => c.String(nullable: false, maxLength: 50));
        }
    }
}
