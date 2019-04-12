namespace PuntoDeVenta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class numclielistanegr : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ListaNegra", "NumCliente", c => c.String(maxLength: 30));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ListaNegra", "NumCliente");
        }
    }
}
