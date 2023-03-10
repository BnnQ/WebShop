using Homework.Data.Entities;
using Homework.Utils;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Homework.Data
{
    public class ShopContext : IdentityDbContext<User>
    {
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<ProductImage> ProductImages { get; set; } = null!;
        public DbSet<Manufacturer> Manufacturers { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<BannerImage> Banners { get; set; } = null!;

        public ShopContext(DbContextOptions options) : base(options)
        {
            //empty
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.SeedRoles()
                   .SeedUsers()
                   .SeedUserRoles();

            builder.Entity<BannerImage>()
                   .HasOne(banner => banner.AssociatedProduct)
                   .WithMany(product => product.AssociatedBanners)
                   .HasForeignKey(banner => banner.AssociatedProductId)
                   .OnDelete(DeleteBehavior.NoAction);
            builder.Entity<BannerImage>(banner =>
            {
                banner.HasKey(banner => banner.Id);
                banner.HasCheckConstraint("CK_Banners_FilePath", "[FilePath] != ''");

                banner.Property(banner => banner.Id)
                     .HasColumnOrder(1)
                     .UseIdentityColumn()
                     .IsRequired();

                banner.Property(banner => banner.FilePath)
                      .HasColumnOrder(2)
                      .HasMaxLength(255)
                      .IsRequired();
            });

            builder.Entity<ProductImage>()
                        .HasOne(image => image.Product)
                        .WithMany(product => product.Images)
                        .HasForeignKey(image => image.ProductId)
                        .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<ProductImage>(image =>
            {
                image.HasKey(image => image.Id);
                image.HasCheckConstraint("CK_ProductImages_FilePath", "[FilePath] != ''");

                image.Property(image => image.Id)
                     .HasColumnOrder(1)
                     .UseIdentityColumn()
                     .IsRequired();

                image.Property(image => image.FilePath)
                     .HasColumnOrder(2)
                     .HasColumnType("nvarchar(255)")
                     .HasDefaultValue("/media/Images/stub.jpg")
                     .HasMaxLength(255);
            });

            builder.Entity<Product>()
                   .HasMany(product => product.Images)
                   .WithOne(image => image.Product)
                   .HasForeignKey(image => image.ProductId);
            builder.Entity<Product>()
                   .HasOne(product => product.Category)
                   .WithMany(category => category.Products)
                   .HasForeignKey(product => product.CategoryId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Product>()
                   .HasOne(product => product.Manufacturer)
                   .WithMany(manufacturer => manufacturer.Products)
                   .HasForeignKey(product => product.ManufacturerId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Product>(product =>
            {
                product.HasKey(product => product.Id);
                product.HasCheckConstraint("CK_Products_Title", "[Title] != ''");

                product.Property(product => product.Id)
                       .HasColumnOrder(1)
                       .UseIdentityColumn()
                       .IsRequired();

                product.Property(product => product.Title)
                       .HasColumnOrder(2)
                       .HasColumnType("nvarchar(255)")
                       .HasMaxLength(255)
                       .IsRequired();
                product.HasIndex(product => product.Title).IsUnique();

                product.Property(product => product.Rating)
                .HasColumnOrder(3)
                .HasConversion(rating => rating, rating => Math.Max(0, Math.Min(rating, 5)))
                .IsRequired();

                product.Property(product => product.Price)
                       .HasColumnOrder(4)
                       .HasColumnType("money")
                       .HasConversion(price => price, price => Math.Max(1, Math.Min(1000000, price)));

                product.Property(product => product.Count)
                       .HasColumnOrder(5)
                       .HasDefaultValue(0)
                       .HasConversion(count => count, count => Math.Max(0, Math.Min(1000000, count)));
            });
            builder.Entity<Manufacturer>()
                   .HasMany(manufacturer => manufacturer.Products)
                   .WithOne(product => product.Manufacturer)
                   .HasForeignKey(product => product.ManufacturerId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Manufacturer>(manufacturer =>
            {
                manufacturer.HasKey(manufacturer => manufacturer.Id);
                manufacturer.HasCheckConstraint("CK_Manufacturers_Name", "[Name] != ''");

                manufacturer.Property(manufacturer => manufacturer.Id)
                       .HasColumnOrder(1)
                       .UseIdentityColumn()
                       .IsRequired();

                manufacturer.Property(manufacturer => manufacturer.Name)
                       .HasColumnOrder(2)
                       .HasColumnType("nvarchar(255)")
                       .HasMaxLength(255)
                       .IsRequired();
            });

            builder.Entity<Category>()
                   .HasOne(category => category.ParentCategory)
                   .WithMany(category => category.ChildCategories)
                   .HasForeignKey(category => category.ParentCategoryId)
                   .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Category>()
                   .HasMany(category => category.Products)
                   .WithOne(product => product.Category)
                   .HasForeignKey(product => product.CategoryId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Category>(category =>
            {
                category.HasKey(category => category.Id);
                category.HasCheckConstraint("CK_Categories_Name", "[Name] != ''");

                category.Property(category => category.Id)
                        .HasColumnOrder(1)
                        .UseIdentityColumn()
                        .IsRequired();

                category.Property(category => category.Name)
                        .HasColumnOrder(2)
                        .HasColumnType("nvarchar(255)")
                        .HasMaxLength(255)
                        .IsRequired();

                category.Property(category => category.UnitName)
                        .HasColumnOrder(3)
                        .HasColumnType("nvarchar(128)")
                        .HasMaxLength(128)
                        .IsRequired();

                category.HasIndex(category => category.Name).IsUnique();
            });

            builder.Entity<Category>().HasData(
                    new Category
                    {
                        Id = 1,
                        Name = "Smartphones",
                        UnitName = "Smartphone"
                    });

            builder.Entity<Manufacturer>().HasData(
                    new Manufacturer
                    {
                        Id = 1,
                        Name = "Samsung"
                    },
                    new Manufacturer
                    {
                        Id = 2,
                        Name = "TECNO"
                    },
                    new Manufacturer
                    {
                        Id = 3,
                        Name = "Blackview"
                    },
                    new Manufacturer
                    {
                        Id = 4,
                        Name = "Apple"
                    },
                    new Manufacturer
                    {
                        Id = 5,
                        Name = "OnePlus"
                    },
                    new Manufacturer
                    {
                        Id = 6,
                        Name = "TCL"
                    });

            builder.Entity<Product>().HasData(
                    new Product
                    {
                        Id = 1,
                        Title = "Galaxy A33 5G 6/128Gb Black",
                        ManufacturerId = 1,
                        CategoryId = 1,
                        Rating = 5,
                        Price = 11999,
                        Count = 50
                    },
                    new Product
                    {
                        Id = 2,
                        Title = "POVA-2 4/128Gb Dazzle Black",
                        ManufacturerId = 2,
                        CategoryId = 1,
                        Rating = 3,
                        Price = 6999,
                        Count = 100
                    },
                    new Product
                    {
                        Id = 3,
                        Title = "Galaxy A23 6/128Gb LTE Black",
                        ManufacturerId = 1,
                        CategoryId = 1,
                        Rating = 4,
                        Price = 8999,
                        Count = 75
                    },
                    new Product
                    {
                        Id = 4,
                        Title = "BV5900 3/32GB DS Orange",
                        ManufacturerId = 3,
                        CategoryId = 1,
                        Rating = 3,
                        Price = 4799,
                        Count = 100
                    },
                    new Product
                    {
                        Id = 5,
                        Title = "iPhone 11 64GB White",
                        ManufacturerId = 4,
                        CategoryId = 1,
                        Rating = 5,
                        Price = 22499,
                        Count = 100
                    },
                    new Product
                    {
                        Id = 6,
                        Title = "Nord AC2003 8/128Gb Gray Onyx",
                        ManufacturerId = 5,
                        CategoryId = 1,
                        Rating = 4,
                        Price = 13999,
                        Count = 150
                    },
                    new Product
                    {
                        Id = 7,
                        Title = "POP 2F 1/16GB Dawn Blue",
                        ManufacturerId = 2,
                        CategoryId = 1,
                        Rating = 2,
                        Price = 2399,
                        Count = 200
                    },
                    new Product
                    {
                        Id = 8,
                        Title = "Galaxy S22 8/128 Green",
                        ManufacturerId = 1,
                        CategoryId = 1,
                        Rating = 5,
                        Price = 33499,
                        Count = 50
                    },
                    new Product
                    {
                        Id = 9,
                        Title = "20L 4/128Gb Eclipse Black",
                        ManufacturerId = 6,
                        CategoryId = 1,
                        Rating = 3,
                        Price = 7599,
                        Count = 75
                    },
                    new Product
                    {
                        Id = 10,
                        Title = "Galaxy A03 Core Ceramic Black",
                        ManufacturerId = 1,
                        CategoryId = 1,
                        Rating = 2,
                        Price = 4099,
                        Count = 100
                    },
                    new Product
                    {
                        Id = 11,
                        Title = "Galaxy A53 5G 6/128Gb White",
                        ManufacturerId = 1,
                        CategoryId = 1,
                        Rating = 4,
                        Price = 15999,
                        Count = 5
                    },
                    new Product
                    {
                        Id = 12,
                        Title = "iPhone 14 128GB Midnight",
                        ManufacturerId = 4,
                        CategoryId = 1,
                        Rating = 5,
                        Price = 36999,
                        Count = 0
                    },
                    new Product
                    {
                        Id = 13,
                        Title = "iPhone 14 Pro Max 128GB Deep Purple",
                        ManufacturerId = 4,
                        CategoryId = 1,
                        Rating = 5,
                        Price = 54999,
                        Count = 1
                    },
                    new Product
                    {
                        Id = 14,
                        Title = "Spark Go 2022 2/32Gb NFC 2SIM Ice Silver",
                        ManufacturerId = 2,
                        CategoryId = 1,
                        Rating = 2,
                        Price = 3799,
                        Count = 145
                    },
                    new Product
                    {
                        Id = 15,
                        Title = "Galaxy A04 3/32Gb Copper",
                        ManufacturerId = 1,
                        CategoryId = 1,
                        Rating = 2,
                        Price = 4555,
                        Count = 100
                    },
                    new Product
                    {
                        Id = 16,
                        Title = "POVA NEO-2 4/64Gb Cyber Blue",
                        ManufacturerId = 2,
                        CategoryId = 1,
                        Rating = 5,
                        Price = 11999,
                        Count = 50
                    },
                    new Product
                    {
                        Id = 17,
                        Title = "BV4900 3/32GB Green",
                        ManufacturerId = 3,
                        CategoryId = 1,
                        Rating = 2,
                        Price = 5999,
                        Count = 120
                    },
                    new Product
                    {
                        Id = 18,
                        Title = "POP 6 Pro 2/32Gb Polar Black",
                        ManufacturerId = 2,
                        CategoryId = 1,
                        Rating = 2,
                        Price = 3999,
                        Count = 75
                    },
                    new Product
                    {
                        Id = 19,
                        Title = "A55 3/16Gb Summer Mojito",
                        ManufacturerId = 3,
                        CategoryId = 1,
                        Rating = 2,
                        Price = 3799,
                        Count = 150
                    },
                    new Product
                    {
                        Id = 20,
                        Title = "POP 5 LTE 2/32Gb Deepsea Luster",
                        ManufacturerId = 2,
                        CategoryId = 1,
                        Rating = 2,
                        Price = 3499,
                        Count = 30
                    });

            builder.Entity<ProductImage>().HasData(
                    new ProductImage
                    {
                        Id = 1,
                        FilePath = "/media/Images/Product/1_product-image-1.jpg",
                        ProductId = 1
                    },
                    new ProductImage
                    {
                        Id = 2,
                        FilePath = "/media/Images/Product/1_product-image-2.jpg",
                        ProductId = 1
                    },
                    new ProductImage
                    {
                        Id = 3,
                        FilePath = "/media/Images/Product/2_product-image-1.jpg",
                        ProductId = 2
                    },
                    new ProductImage
                    {
                        Id = 4,
                        FilePath = "/media/Images/Product/2_product-image-2.jpg",
                        ProductId = 2
                    },
                    new ProductImage
                    {
                        Id = 5,
                        FilePath = "/media/Images/Product/3_product-image-1.jpg",
                        ProductId = 3
                    },
                    new ProductImage
                    {
                        Id = 6,
                        FilePath = "/media/Images/Product/3_product-image-2.jpg",
                        ProductId = 3
                    },
                    new ProductImage
                    {
                        Id = 7,
                        FilePath = "/media/Images/Product/4_product-image-1.jpg",
                        ProductId = 4
                    },
                    new ProductImage
                    {
                        Id = 8,
                        FilePath = "/media/Images/Product/4_product-image-2.jpg",
                        ProductId = 4
                    },
                    new ProductImage
                    {
                        Id = 9,
                        FilePath = "/media/Images/Product/5_product-image-1.jpg",
                        ProductId = 5
                    },
                    new ProductImage
                    {
                        Id = 10,
                        FilePath = "/media/Images/Product/5_product-image-2.jpg",
                        ProductId = 5
                    },
                    new ProductImage
                    {
                        Id = 11,
                        FilePath = "/media/Images/Product/6_product-image-1.jpg",
                        ProductId = 6
                    },
                    new ProductImage
                    {
                        Id = 12,
                        FilePath = "/media/Images/Product/6_product-image-2.jpg",
                        ProductId = 6
                    },
                    new ProductImage
                    {
                        Id = 13,
                        FilePath = "/media/Images/Product/7_product-image-1.jpg",
                        ProductId = 7
                    },
                    new ProductImage
                    {
                        Id = 14,
                        FilePath = "/media/Images/Product/7_product-image-2.jpg",
                        ProductId = 7
                    },
                    new ProductImage
                    {
                        Id = 15,
                        FilePath = "/media/Images/Product/8_product-image-1.jpg",
                        ProductId = 8
                    },
                    new ProductImage
                    {
                        Id = 16,
                        FilePath = "/media/Images/Product/8_product-image-2.jpg",
                        ProductId = 8
                    },
                    new ProductImage
                    {
                        Id = 17,
                        FilePath = "/media/Images/Product/9_product-image-1.jpg",
                        ProductId = 9
                    },
                    new ProductImage
                    {
                        Id = 18,
                        FilePath = "/media/Images/Product/9_product-image-2.jpg",
                        ProductId = 9
                    },
                    new ProductImage
                    {
                        Id = 19,
                        FilePath = "/media/Images/Product/10_product-image-1.jpg",
                        ProductId = 10
                    },
                    new ProductImage
                    {
                        Id = 20,
                        FilePath = "/media/Images/Product/10_product-image-2.jpg",
                        ProductId = 10
                    },
                    new ProductImage
                    {
                        Id = 21,
                        FilePath = "/media/Images/Product/11_product-image-1.jpg",
                        ProductId = 11
                    },
                    new ProductImage
                    {
                        Id = 22,
                        FilePath = "/media/Images/Product/11_product-image-2.jpg",
                        ProductId = 11
                    },
                    new ProductImage
                    {
                        Id = 23,
                        FilePath = "/media/Images/Product/12_product-image-1.jpg",
                        ProductId = 12
                    },
                    new ProductImage
                    {
                        Id = 24,
                        FilePath = "/media/Images/Product/12_product-image-2.jpg",
                        ProductId = 12
                    },
                    new ProductImage
                    {
                        Id = 25,
                        FilePath = "/media/Images/Product/13_product-image-1.jpg",
                        ProductId = 13
                    },
                    new ProductImage
                    {
                        Id = 26,
                        FilePath = "/media/Images/Product/13_product-image-2.jpg",
                        ProductId = 13
                    },
                    new ProductImage
                    {
                        Id = 27,
                        FilePath = "/media/Images/Product/14_product-image-1.jpg",
                        ProductId = 14
                    },
                    new ProductImage
                    {
                        Id = 28,
                        FilePath = "/media/Images/Product/14_product-image-2.jpg",
                        ProductId = 14
                    },
                    new ProductImage
                    {
                        Id = 29,
                        FilePath = "/media/Images/Product/15_product-image-1.jpg",
                        ProductId = 15
                    },
                    new ProductImage
                    {
                        Id = 30,
                        FilePath = "/media/Images/Product/15_product-image-2.jpg",
                        ProductId = 15
                    },
                    new ProductImage
                    {
                        Id = 31,
                        FilePath = "/media/Images/Product/16_product-image-1.jpg",
                        ProductId = 16
                    },
                    new ProductImage
                    {
                        Id = 32,
                        FilePath = "/media/Images/Product/16_product-image-2.jpg",
                        ProductId = 16
                    },
                    new ProductImage
                    {
                        Id = 33,
                        FilePath = "/media/Images/Product/17_product-image-1.jpg",
                        ProductId = 17
                    },
                    new ProductImage
                    {
                        Id = 34,
                        FilePath = "/media/Images/Product/17_product-image-2.jpg",
                        ProductId = 17
                    },
                    new ProductImage
                    {
                        Id = 35,
                        FilePath = "/media/Images/Product/18_product-image-1.jpg",
                        ProductId = 18
                    },
                    new ProductImage
                    {
                        Id = 36,
                        FilePath = "/media/Images/Product/18_product-image-2.jpg",
                        ProductId = 18
                    },
                    new ProductImage
                    {
                        Id = 37,
                        FilePath = "/media/Images/Product/19_product-image-1.jpg",
                        ProductId = 19
                    },
                    new ProductImage
                    {
                        Id = 38,
                        FilePath = "/media/Images/Product/19_product-image-2.jpg",
                        ProductId = 19
                    },
                    new ProductImage
                    {
                        Id = 39,
                        FilePath = "/media/Images/Product/20_product-image-1.jpg",
                        ProductId = 20
                    },
                    new ProductImage
                    {
                        Id = 40,
                        FilePath = "/media/Images/Product/20_product-image-2.jpg",
                        ProductId = 20
                    });

            builder.Entity<BannerImage>().HasData(
                    new BannerImage
                    {
                        Id = 1,
                        FilePath = "/media/Images/Banners/banner1.jpg"
                    },
                    new BannerImage
                    {
                        Id = 2,
                        FilePath = "/media/Images/Banners/banner2.jpg"
                    },
                    new BannerImage
                    {
                        Id = 3,
                        FilePath = "/media/Images/Banners/banner3.jpg"
                    },
                    new BannerImage
                    {
                        Id = 4,
                        FilePath = "/media/Images/Banners/banner4.jpg"
                    },
                    new BannerImage
                    {
                        Id = 5,
                        FilePath = "/media/Images/Banners/banner5.jpg"
                    });

            base.OnModelCreating(builder);
        }
    }
}