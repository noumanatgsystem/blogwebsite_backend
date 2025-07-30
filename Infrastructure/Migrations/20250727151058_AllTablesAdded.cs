using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AllTablesAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BlogPost",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "NVARCHAR(500)", maxLength: 500, nullable: false),
                    Slug = table.Column<string>(type: "NVARCHAR(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "NVARCHAR(1000)", maxLength: 1000, nullable: false),
                    FileUrl = table.Column<string>(type: "NVARCHAR(1000)", maxLength: 1000, nullable: false),
                    FileKey = table.Column<string>(type: "NVARCHAR(1000)", maxLength: 1000, nullable: false),
                    StyleUrl = table.Column<string>(type: "NVARCHAR(1000)", maxLength: 1000, nullable: true),
                    StyleKey = table.Column<string>(type: "NVARCHAR(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<string>(type: "NVARCHAR(20)", maxLength: 20, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsDeclined = table.Column<bool>(type: "bit", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    IsPending = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogPost", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlogPost_AppUser_UserId",
                        column: x => x.UserId,
                        principalTable: "AppUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "NVARCHAR(500)", maxLength: 500, nullable: false),
                    Slug = table.Column<string>(type: "NVARCHAR(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "NVARCHAR(1000)", maxLength: 1000, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsDeclined = table.Column<bool>(type: "bit", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    IsPending = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "VARCHAR(255)", maxLength: 255, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsDeclined = table.Column<bool>(type: "bit", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    IsPending = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BlogPostCategoryRel",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BlogPostId = table.Column<long>(type: "bigint", nullable: false),
                    CategoryId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogPostCategoryRel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlogPostCategoryRel_BlogPost_BlogPostId",
                        column: x => x.BlogPostId,
                        principalTable: "BlogPost",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BlogPostCategoryRel_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BlogPostTagRel",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BlogPostId = table.Column<long>(type: "bigint", nullable: false),
                    TagId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogPostTagRel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlogPostTagRel_BlogPost_BlogPostId",
                        column: x => x.BlogPostId,
                        principalTable: "BlogPost",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BlogPostTagRel_Category_TagId",
                        column: x => x.TagId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlogPost_UserId",
                table: "BlogPost",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogPostCategoryRel_BlogPostId",
                table: "BlogPostCategoryRel",
                column: "BlogPostId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogPostCategoryRel_CategoryId",
                table: "BlogPostCategoryRel",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogPostTagRel_BlogPostId",
                table: "BlogPostTagRel",
                column: "BlogPostId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogPostTagRel_TagId",
                table: "BlogPostTagRel",
                column: "TagId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlogPostCategoryRel");

            migrationBuilder.DropTable(
                name: "BlogPostTagRel");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "BlogPost");

            migrationBuilder.DropTable(
                name: "Category");
        }
    }
}
