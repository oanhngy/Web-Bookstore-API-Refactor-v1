using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BookstoreWeb.Application.Domain;
namespace BookstoreWeb.Infrastructure.Data;

//là entry point for all db operations, inherits IdentityDbContext để incllude ASP.NET identity table vì Order.USerID là FK to Identity user table
public class BookstoreContext : IdentityDbContext<IdentityUser>
{
    public BookstoreContext(DbContextOptions<BookstoreContext> options):base(options)
    {
        //   
    }

    //each DbSet map to a tablle in db
    public DbSet<Category> Categories {get; set;}
    public DbSet<Product> Products {get; set;}
    public DbSet<ProductImage> ProductImages {get; set;}
    public DbSet<Order> Orders {get; set;}
    public DbSet<OrderDetail> OrderDetails {get; set;}

    //configure entity relationship that EF core cannot auto infer
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        base.OnModelCreating(modelBuilder); //set up Identity table
        modelBuilder.Entity<Product>()
            .HasMany(p=>p.ProductImages)
            .WithOne(Ping=>Ping.Product)
            .HasForeignKey(Ping=>Ping.ProductID);

        //Order belong to IdentityUSer=UserID, deleting a user is NOT auto-delete their orders
        modelBuilder.Entity<Order>()
            .HasOne<IdentityUser>()
            .WithMany()
            .HasForeignKey(o=>o.UserID)
            .OnDelete(DeleteBehavior.NoAction);
    }
}