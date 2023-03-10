using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Homework.Migrations.LocalDatabase
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    YearOfBirth = table.Column<int>(type: "int", nullable: false),
                    Balance = table.Column<double>(type: "float", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UnitName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ParentCategoryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                    table.CheckConstraint("CK_Categories_Name", "[Name] != ''");
                    table.ForeignKey(
                        name: "FK_Categories_Categories_ParentCategoryId",
                        column: x => x.ParentCategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Manufacturers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Manufacturers", x => x.Id);
                    table.CheckConstraint("CK_Manufacturers_Name", "[Name] != ''");
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<double>(type: "money", nullable: false),
                    Count = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    ManufacturerId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.CheckConstraint("CK_Products_Title", "[Title] != ''");
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Products_Manufacturers_ManufacturerId",
                        column: x => x.ManufacturerId,
                        principalTable: "Manufacturers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Banners",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FilePath = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    AssociatedProductId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Banners", x => x.Id);
                    table.CheckConstraint("CK_Banners_FilePath", "[FilePath] != ''");
                    table.ForeignKey(
                        name: "FK_Banners_Products_AssociatedProductId",
                        column: x => x.AssociatedProductId,
                        principalTable: "Products",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProductImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FilePath = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false, defaultValue: "/media/Images/stub.jpg"),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductImages", x => x.Id);
                    table.CheckConstraint("CK_ProductImages_FilePath", "[FilePath] != ''");
                    table.ForeignKey(
                        name: "FK_ProductImages_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "c7b013f0-5201-4317-abd8-c211f91b7330", "2", "Customer", "CUSTOMER" },
                    { "fab4fac1-c546-41de-aebc-a14da6895711", "1", "Admin", "ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Balance", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName", "YearOfBirth" },
                values: new object[] { "b74ddd14-6340-4840-95c2-db12554843e5", 0, 0.0, "b654e375-2a69-48f8-bf14-a77a2baed553", "admin@gmail.com", false, false, null, "ADMIN@GMAIL.COM", "ADMIN", "AQAAAAEAACcQAAAAECNjWwHJZq65ZaclLKnMelKbVlqfrMNM7b74TzmWlnbH84KveiYmAhhB/aDCkLTdrA==", "+380687507133", false, "8fd48d4a-9fb9-4f24-b33b-90ee8b9f8007", false, "admin", 0 });

            migrationBuilder.InsertData(
                table: "Banners",
                columns: new[] { "Id", "AssociatedProductId", "FilePath" },
                values: new object[,]
                {
                    { 1, null, "/media/Images/Banners/banner1.jpg" },
                    { 2, null, "/media/Images/Banners/banner2.jpg" },
                    { 3, null, "/media/Images/Banners/banner3.jpg" },
                    { 4, null, "/media/Images/Banners/banner4.jpg" },
                    { 5, null, "/media/Images/Banners/banner5.jpg" }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name", "ParentCategoryId", "UnitName" },
                values: new object[] { 1, "Smartphones", null, "Smartphone" });

            migrationBuilder.InsertData(
                table: "Manufacturers",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Samsung" },
                    { 2, "TECNO" },
                    { 3, "Blackview" },
                    { 4, "Apple" },
                    { 5, "OnePlus" },
                    { 6, "TCL" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "fab4fac1-c546-41de-aebc-a14da6895711", "b74ddd14-6340-4840-95c2-db12554843e5" });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "Count", "ManufacturerId", "Price", "Rating", "Title" },
                values: new object[,]
                {
                    { 1, 1, 50, 1, 11999.0, 5, "Galaxy A33 5G 6/128Gb Black" },
                    { 2, 1, 100, 2, 6999.0, 3, "POVA-2 4/128Gb Dazzle Black" },
                    { 3, 1, 75, 1, 8999.0, 4, "Galaxy A23 6/128Gb LTE Black" },
                    { 4, 1, 100, 3, 4799.0, 3, "BV5900 3/32GB DS Orange" },
                    { 5, 1, 100, 4, 22499.0, 5, "iPhone 11 64GB White" },
                    { 6, 1, 150, 5, 13999.0, 4, "Nord AC2003 8/128Gb Gray Onyx" },
                    { 7, 1, 200, 2, 2399.0, 2, "POP 2F 1/16GB Dawn Blue" },
                    { 8, 1, 50, 1, 33499.0, 5, "Galaxy S22 8/128 Green" },
                    { 9, 1, 75, 6, 7599.0, 3, "20L 4/128Gb Eclipse Black" },
                    { 10, 1, 100, 1, 4099.0, 2, "Galaxy A03 Core Ceramic Black" },
                    { 11, 1, 5, 1, 15999.0, 4, "Galaxy A53 5G 6/128Gb White" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "ManufacturerId", "Price", "Rating", "Title" },
                values: new object[] { 12, 1, 4, 36999.0, 5, "iPhone 14 128GB Midnight" });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "Count", "ManufacturerId", "Price", "Rating", "Title" },
                values: new object[,]
                {
                    { 13, 1, 1, 4, 54999.0, 5, "iPhone 14 Pro Max 128GB Deep Purple" },
                    { 14, 1, 145, 2, 3799.0, 2, "Spark Go 2022 2/32Gb NFC 2SIM Ice Silver" },
                    { 15, 1, 100, 1, 4555.0, 2, "Galaxy A04 3/32Gb Copper" },
                    { 16, 1, 50, 2, 11999.0, 5, "POVA NEO-2 4/64Gb Cyber Blue" },
                    { 17, 1, 120, 3, 5999.0, 2, "BV4900 3/32GB Green" },
                    { 18, 1, 75, 2, 3999.0, 2, "POP 6 Pro 2/32Gb Polar Black" },
                    { 19, 1, 150, 3, 3799.0, 2, "A55 3/16Gb Summer Mojito" },
                    { 20, 1, 30, 2, 3499.0, 2, "POP 5 LTE 2/32Gb Deepsea Luster" }
                });

            migrationBuilder.InsertData(
                table: "ProductImages",
                columns: new[] { "Id", "FilePath", "ProductId" },
                values: new object[,]
                {
                    { 1, "/media/Images/Product/1_product-image-1.jpg", 1 },
                    { 2, "/media/Images/Product/1_product-image-2.jpg", 1 },
                    { 3, "/media/Images/Product/2_product-image-1.jpg", 2 },
                    { 4, "/media/Images/Product/2_product-image-2.jpg", 2 },
                    { 5, "/media/Images/Product/3_product-image-1.jpg", 3 },
                    { 6, "/media/Images/Product/3_product-image-2.jpg", 3 },
                    { 7, "/media/Images/Product/4_product-image-1.jpg", 4 },
                    { 8, "/media/Images/Product/4_product-image-2.jpg", 4 },
                    { 9, "/media/Images/Product/5_product-image-1.jpg", 5 },
                    { 10, "/media/Images/Product/5_product-image-2.jpg", 5 },
                    { 11, "/media/Images/Product/6_product-image-1.jpg", 6 },
                    { 12, "/media/Images/Product/6_product-image-2.jpg", 6 },
                    { 13, "/media/Images/Product/7_product-image-1.jpg", 7 },
                    { 14, "/media/Images/Product/7_product-image-2.jpg", 7 },
                    { 15, "/media/Images/Product/8_product-image-1.jpg", 8 },
                    { 16, "/media/Images/Product/8_product-image-2.jpg", 8 },
                    { 17, "/media/Images/Product/9_product-image-1.jpg", 9 },
                    { 18, "/media/Images/Product/9_product-image-2.jpg", 9 },
                    { 19, "/media/Images/Product/10_product-image-1.jpg", 10 },
                    { 20, "/media/Images/Product/10_product-image-2.jpg", 10 },
                    { 21, "/media/Images/Product/11_product-image-1.jpg", 11 },
                    { 22, "/media/Images/Product/11_product-image-2.jpg", 11 },
                    { 23, "/media/Images/Product/12_product-image-1.jpg", 12 },
                    { 24, "/media/Images/Product/12_product-image-2.jpg", 12 },
                    { 25, "/media/Images/Product/13_product-image-1.jpg", 13 },
                    { 26, "/media/Images/Product/13_product-image-2.jpg", 13 },
                    { 27, "/media/Images/Product/14_product-image-1.jpg", 14 },
                    { 28, "/media/Images/Product/14_product-image-2.jpg", 14 },
                    { 29, "/media/Images/Product/15_product-image-1.jpg", 15 },
                    { 30, "/media/Images/Product/15_product-image-2.jpg", 15 },
                    { 31, "/media/Images/Product/16_product-image-1.jpg", 16 },
                    { 32, "/media/Images/Product/16_product-image-2.jpg", 16 },
                    { 33, "/media/Images/Product/17_product-image-1.jpg", 17 },
                    { 34, "/media/Images/Product/17_product-image-2.jpg", 17 },
                    { 35, "/media/Images/Product/18_product-image-1.jpg", 18 },
                    { 36, "/media/Images/Product/18_product-image-2.jpg", 18 },
                    { 37, "/media/Images/Product/19_product-image-1.jpg", 19 },
                    { 38, "/media/Images/Product/19_product-image-2.jpg", 19 },
                    { 39, "/media/Images/Product/20_product-image-1.jpg", 20 },
                    { 40, "/media/Images/Product/20_product-image-2.jpg", 20 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Banners_AssociatedProductId",
                table: "Banners",
                column: "AssociatedProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                table: "Categories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ParentCategoryId",
                table: "Categories",
                column: "ParentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_ProductId",
                table: "ProductImages",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_ManufacturerId",
                table: "Products",
                column: "ManufacturerId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Title",
                table: "Products",
                column: "Title",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Banners");

            migrationBuilder.DropTable(
                name: "ProductImages");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Manufacturers");
        }
    }
}
