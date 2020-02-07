using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Boticario.Api.Models;
using Boticario.Api.Models.Enums;

namespace Boticario.Api.Context
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {           
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

            base.OnModelCreating(builder);
        }

        public DbSet<SalesModel> Sales { get; set; }
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
