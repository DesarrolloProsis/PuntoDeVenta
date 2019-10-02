using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.Infrastructure.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PuntoDeVenta.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<AppDbContext>(new CreateDatabaseIfNotExists<AppDbContext>());
        }

        public DbSet<Clientes> Clientes { get; set; }
        public DbSet<CuentasTelepeaje> CuentasTelepeajes { get; set; }
        public DbSet<Tags> Tags { get; set; }
        public DbSet<OperacionesSerBIpagos> OperacionesSerBIpagos { get; set; }
        public DbSet<CortesCajero> CortesCajeros { get; set; }
        public DbSet<OperacionesCajero> OperacionesCajeros { get; set; }
        public DbSet<MontosRecargables> MontosRecargables { get; set; }
        public DbSet<Parametrizable> Parametrizables { get; set; }
        public DbSet<Historico> Historicos { get; set; }
        public DbSet<ListaNegra> ListaNegras { get; set; }
        public DbSet<HistoricoListas> HistoricoListas { get; set; }
        public DbSet<AmountConfiguration> AmountConfigurations { get; set; }
        public DbSet<Carriles> Carriles { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<TipoVehiculo> TipoVehiculos { get; set; }
        public DbSet<Operadores> Operadores { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new CuentasTelepeajeMap());
            modelBuilder.Configurations.Add(new HistoricoMap());
            modelBuilder.Configurations.Add(new TagsMap());
            modelBuilder.Configurations.Add(new OperacionesCajeroMap());
            modelBuilder.Configurations.Add(new CarrilesMap());
            modelBuilder.Configurations.Add(new LocationMap());
            modelBuilder.Configurations.Add(new TipoVehiculoMap());
            modelBuilder.Configurations.Add(new OperadoresMap());
            modelBuilder.Configurations.Add(new ClientesMap());

            base.OnModelCreating(modelBuilder);
        }

        public class CarrilesMap : EntityTypeConfiguration<Carriles>
        {
            public CarrilesMap()
            {
                ToTable("Carriles");
                Property(x => x.Carril).HasMaxLength(3);
                HasKey(x => x.Carril);
            }

        }

        public class ClientesMap : EntityTypeConfiguration<Clientes>
        {
            public ClientesMap()
            {
                Property(x => x.NumCliente).HasMaxLength(20);
                Property(x => x.Nombre).HasMaxLength(70);
                Property(x => x.Apellidos).HasMaxLength(70);
                Property(x => x.EmailCliente).HasMaxLength(70);
                Property(x => x.AddressCliente).HasMaxLength(220);
                Property(x => x.PhoneCliente).HasMaxLength(30);
                Property(x => x.IdCajero).HasMaxLength(128);
                Property(x => x.Empresa).HasMaxLength(100);
                Property(x => x.CP).HasMaxLength(15);
                Property(x => x.Pais).HasMaxLength(70);
                Property(x => x.City).HasMaxLength(50);
                Property(x => x.Departamento).HasMaxLength(70);
                Property(x => x.NIT).HasMaxLength(30);
                Property(x => x.PhoneOffice).HasMaxLength(30);
            }

        }


        public class CuentasTelepeajeMap : EntityTypeConfiguration<CuentasTelepeaje>
        {
            public CuentasTelepeajeMap()
            {
                HasRequired<Clientes>(x => x.Clientes).WithMany(x => x.CuentasTelepeajes).HasForeignKey(x => x.ClienteId);
                Property(x => x.SaldoCuenta).HasPrecision(10, 2);
                Property(x => x.SaldoCuenta).IsOptional();
            }

        }

        public class HistoricoMap : EntityTypeConfiguration<Historico>
        {
            public HistoricoMap()
            {
                ToTable("Historico");
                HasKey(x => new { x.Tag, x.Fecha, x.Carril });
                Property(x => x.Tag).HasMaxLength(20);
                HasRequired(x => x.Carriles).WithMany(x => x.Historicos).HasForeignKey(x => x.Carril);
                HasRequired(x => x.TipoVehiculo).WithMany(x => x.Historicos).HasForeignKey(x => x.IdClase);
                HasRequired(x => x.Location).WithMany(x => x.Historicos).HasForeignKey(x => x.IdLocation);
                HasRequired(x => x.Operadores).WithMany(x => x.Historicos).HasForeignKey(x => x.IdOperador);
                Property(x => x.Saldo).HasPrecision(6, 2);
                Property(x => x.Saldo).HasColumnName("Cargo");
                Property(x => x.SaldoAnterior).HasPrecision(10, 2);
                Property(x => x.SaldoActualizado).HasPrecision(10, 2);
                Property(x => x.NumeroCuenta).HasMaxLength(10);
            }
        }


        public class LocationMap : EntityTypeConfiguration<Location>
        {
            public LocationMap()
            {
                ToTable("Location");
                HasKey(x => x.IdLocation);
                Property(x => x.Delegacion).HasMaxLength(30);
                Property(x => x.Plaza).HasMaxLength(10);
            }
        }

        public class OperadoresMap : EntityTypeConfiguration<Operadores>
        {
            public OperadoresMap()
            {
                ToTable("Operadores");
                HasKey(x => x.IdOperador);
                Property(x => x.Operador).HasMaxLength(5);

            }
        }


        public class TagsMap : EntityTypeConfiguration<Tags>
        {
            public TagsMap()
            {
                HasRequired<CuentasTelepeaje>(x => x.CuentasTelepeaje).WithMany(x => x.Tags).HasForeignKey(x => x.CuentaId);
                Property(x => x.SaldoTag).HasPrecision(10,2);
            }
        }
      

        public class OperacionesCajeroMap : EntityTypeConfiguration<OperacionesCajero>
        {
            public OperacionesCajeroMap()
            {
                HasRequired<CortesCajero>(x => x.CortesCajero).WithMany(x => x.OperacionesCajeros).HasForeignKey(x => x.CorteId);

            }
        }


        public class TipoVehiculoMap : EntityTypeConfiguration<TipoVehiculo>
        {
            public TipoVehiculoMap()
            {
                ToTable("TipoVehiculo");
                Property(x => x.IdTipoVehiculo).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
                HasKey(x => x.IdTipoVehiculo);
                Property(x => x.Tipo).HasMaxLength(6);
               

            }
        }

    }
}