﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Boticario.Api.Models;
using Boticario.Api.Models.Enums;
using Microsoft.Extensions.Configuration;

namespace Boticario.Api.Context
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public IConfiguration Configuration { get; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<SalesModel>().ToTable("Sales")
                .Ignore( t => t.IsValid)
                .Ignore(t => t.Message)
                .Property(x => x.Status)        
                .HasConversion(x => (int)x, x => (SalesStatus)x)
                //.HasColumnType("int")
                .IsRequired();

            builder.Entity<LogModel>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();

            base.OnModelCreating(builder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfigurationRoot config = builder.Build();
            optionsBuilder.UseSqlServer(config.GetConnectionString("DefaultConnection"));
        }

        public DbSet<SalesModel> Sales { get; set; }
        public DbSet<LogModel> Log { get; set; }
    }

    ////public class AppDbContext : DbContext
    ////{
    ////    public AppDbContext()
    ////    { }

    ////    protected override void OnModelCreating(ModelBuilder builder)
    ////    {
    ////        builder.Entity<SalesModel>().ToTable("Sales")
    ////            .Ignore(t => t.IsValid)
    ////            .Ignore(t => t.Message)
    ////            .Property(x => x.Status)
    ////            .HasConversion(x => (int)x, x => (SalesStatus)x)
    ////            //.HasColumnType("int")
    ////            .IsRequired();

    ////        base.OnModelCreating(builder);
    ////    }

    ////    public DbSet<SalesModel> Sales { get; set; }
    ////}
}
