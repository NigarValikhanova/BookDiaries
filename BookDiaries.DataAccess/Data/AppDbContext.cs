using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookDiaries.Models.Models;
using System.Reflection.Emit;

namespace BookDiaries.DataAccess.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetail> OrderDetails{ get; set; }
        public DbSet<WishList> WishLists { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<UserCoupon> UserCoupon { get; set; }
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<ContactUser> ContactUsers { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<RatingReview> RatingReviews { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // Include base configurations

            // Seed Categories
            builder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Fantasy" },
                new Category { Id = 2, Name = "SciFi" },
                new Category { Id = 3, Name = "Action" },
                new Category { Id = 4, Name = "Romance" },
                new Category { Id = 5, Name = "Mystery" }
            );
            builder.Entity<Product>().HasData(
               new Product
               {
                   Id = 1,
                   Title = "Pride and Prejudice",
                   Description = "Pride and Prejudice follows the turbulent relationship between Elizabeth Bennet, the daughter of a country gentleman, and Fitzwilliam Darcy, a rich aristocratic landowner. They must overcome the titular sins of pride and prejudice in order to fall in love and marry.",
                   ISBN = "9780192827609",
                   Price = 65,
                   Price50 = 60,
                   Price100 = 55,
                   CategoryId =1,
                   StockQuantity = 50,
                   LanguageId=2,
                   AuthorId=2
               },
               new Product
               {
                   Id = 2,
                   Title = "The Fellowship of the Ring",
                   Description = "The Fellowship of the Ring consists of nine walkers who set out on the quest to destroy the One Ring, in opposition to the nine Black Riders: Frodo Baggins, Sam Gamgee, Merry Brandybuck and Pippin Took; Gandalf; the Men Aragorn and Boromir, son of the Steward of Gondor; the Elf Legolas; and the Dwarf Gimli.",
                   ISBN = "9788845270741",
                   Price = 90,
                   Price50 = 85,
                   Price100 = 80,
                   CategoryId =2,
                   StockQuantity=50,
                   LanguageId = 2,
                   AuthorId=1
               },
               new Product
               {
                   Id = 3,
                   Title = "The Great Gatsby",
                   Description= "The Great Gatsby, novel by American author F. Scott Fitzgerald, published in 1925. It tells the story of Jay Gatsby, a self-made millionaire, and his pursuit of Daisy Buchanan, a wealthy young woman whom he loved in his youth. Set in 1920s New York, the book is narrated by Nick Carraway.",
                   ISBN = "9780199536405",
                   Price = 50,
                   Price50 = 40,
                   Price100 = 35,
                   CategoryId =3,
                   StockQuantity=50,
                   LanguageId = 2,
                   AuthorId = 3
               }
           );
            builder.Entity<Language>().HasData(
               new Language { Id = 1, Name = "Azerbaijani" },
               new Language { Id = 2, Name = "English" },
               new Language { Id = 3, Name = "Turkish" },
               new Language { Id = 4, Name = "Russian" }
           );

            builder.Entity<Author>().HasData(
              new Author { Id = 1, Name = "J.R.R.Tolkien"},
              new Author { Id = 2, Name = "Jane Austen"},
              new Author { Id = 3, Name = "F.Scott Fitzgerald"}
            );

            builder.Entity<UserCoupon>()
               .HasKey(uc => new { uc.UserId, uc.CouponId });

            builder.Entity<UserCoupon>()
                .HasOne(uc => uc.User)
                .WithMany(u => u.UserCoupons)
                .HasForeignKey(uc => uc.UserId);

            builder.Entity<UserCoupon>()
                .HasOne(uc => uc.Coupon)
                .WithMany(c => c.UserCoupons)
                .HasForeignKey(uc => uc.CouponId);

        }
    }
}
