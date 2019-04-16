namespace PuntoDeVenta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addnumcuenta : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ListaNegra", "NumCuenta", c => c.String(maxLength: 30));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ListaNegra", "NumCuenta");
        }
    }
}
