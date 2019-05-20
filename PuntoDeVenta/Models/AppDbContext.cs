using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

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
        public DbSet<AmountSettings> AmountSettings { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {


            modelBuilder.Entity<CuentasTelepeaje>()
                .HasRequired<Clientes>(x => x.Clientes)
                .WithMany(x => x.CuentasTelepeajes)
                .HasForeignKey(x => x.ClienteId);

            modelBuilder.Entity<Tags>()
                .HasRequired<CuentasTelepeaje>(x => x.CuentasTelepeaje)
                .WithMany(x => x.Tags)
                .HasForeignKey(x => x.CuentaId);

            modelBuilder.Entity<OperacionesSerBIpagos>()
                .HasRequired<Tags>(x => x.Tags)
                .WithMany(x => x.OperacionesSerBIpagos)
                .HasForeignKey(x => x.TagId);

            modelBuilder.Entity<OperacionesCajero>()
                .HasRequired<CortesCajero>(x => x.CortesCajero)
                .WithMany(x => x.OperacionesCajeros)
                .HasForeignKey(x => x.CorteId);

            base.OnModelCreating(modelBuilder);
        }
    }
}