namespace PuntoDeVenta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cientecampos : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clientes", "Empresa", c => c.String());
            AddColumn("dbo.Clientes", "CP", c => c.String());
            AddColumn("dbo.Clientes", "Pais", c => c.String());
            AddColumn("dbo.Clientes", "City", c => c.String());
            AddColumn("dbo.Clientes", "Departamento", c => c.String());
            AddColumn("dbo.Clientes", "NIT", c => c.String());
            AddColumn("dbo.Clientes", "PhoneOffice", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Clientes", "PhoneOffice");
            DropColumn("dbo.Clientes", "NIT");
            DropColumn("dbo.Clientes", "Departamento");
            DropColumn("dbo.Clientes", "City");
            DropColumn("dbo.Clientes", "Pais");
            DropColumn("dbo.Clientes", "CP");
            DropColumn("dbo.Clientes", "Empresa");
        }
    }
}
