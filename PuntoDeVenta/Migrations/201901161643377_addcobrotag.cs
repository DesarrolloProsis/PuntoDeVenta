namespace PuntoDeVenta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addcobrotag : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OperacionesCajeroes", "CobroTag", c => c.Double());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OperacionesCajeroes", "CobroTag");
        }
    }
}
