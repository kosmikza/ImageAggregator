using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImageAggregatorAPI.Migrations
{
    public partial class CreateDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ImageBlobs",
                columns: table => new
                {
                    ImageBlobId = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Blob = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageBlobs", x => x.ImageBlobId);
                });

            migrationBuilder.CreateTable(
                name: "ImageLocationMaps",
                columns: table => new
                {
                    ImageLocationMapId = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LocationId = table.Column<long>(nullable: false),
                    ImageId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageLocationMaps", x => x.ImageLocationMapId);
                });

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    ImageId = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImageName = table.Column<string>(nullable: false),
                    ImageWebURL = table.Column<string>(nullable: true),
                    ImageBlobId = table.Column<long>(nullable: true),
                    Height = table.Column<int>(nullable: false),
                    Width = table.Column<int>(nullable: false),
                    ImageURLPrefix = table.Column<string>(nullable: true),
                    ImageURLSuffix = table.Column<string>(nullable: true),
                    FourSquareVenueId = table.Column<string>(nullable: true),
                    GooglePlaceId = table.Column<string>(nullable: true),
                    FourSquareImageId = table.Column<string>(nullable: true),
                    GoogleImageId = table.Column<string>(nullable: true),
                    ImageApiSource = table.Column<int>(nullable: false),
                    Latitude = table.Column<double>(nullable: true),
                    Longitude = table.Column<double>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.ImageId);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    LocationId = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LocationName = table.Column<string>(nullable: false),
                    LocationCountryCode = table.Column<string>(nullable: true),
                    LongitudeDecimal = table.Column<double>(nullable: true),
                    LatitudeDecimal = table.Column<double>(nullable: true),
                    ImageLoadingComplete = table.Column<bool>(nullable: false),
                    LastRunStatus = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.LocationId);
                });

            migrationBuilder.CreateTable(
                name: "UserLocationMaps",
                columns: table => new
                {
                    UserLocationMapId = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(nullable: false),
                    LocationId = table.Column<long>(nullable: false),
                    AddedOn = table.Column<DateTime>(nullable: false),
                    RemovedOn = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLocationMaps", x => x.UserLocationMapId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Username = table.Column<string>(nullable: true),
                    PasswordHash = table.Column<byte[]>(nullable: true),
                    PasswordSalt = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImageBlobs");

            migrationBuilder.DropTable(
                name: "ImageLocationMaps");

            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "UserLocationMaps");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
