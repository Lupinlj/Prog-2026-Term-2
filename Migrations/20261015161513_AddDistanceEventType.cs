using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RaceDay.Migrations
{
    /// <inheritdoc />
    public partial class AddDistanceEventType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Organiser_OrganiserId",
                table: "Events");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Organiser",
                table: "Organiser");

            migrationBuilder.RenameTable(
                name: "Organiser",
                newName: "Organisers");

            migrationBuilder.AddColumn<decimal>(
                name: "Distance",
                table: "Events",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "EventType",
                table: "Events",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "CategoryName",
                table: "Categories",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "OrganiserPassword",
                table: "Organisers",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "OrganiserName",
                table: "Organisers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "OrganiserEmail",
                table: "Organisers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Organisers",
                table: "Organisers",
                column: "OrganiserId");

            migrationBuilder.CreateIndex(
                name: "IX_Organisers_OrganiserEmail",
                table: "Organisers",
                column: "OrganiserEmail",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Organisers_OrganiserId",
                table: "Events",
                column: "OrganiserId",
                principalTable: "Organisers",
                principalColumn: "OrganiserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Organisers_OrganiserId",
                table: "Events");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Organisers",
                table: "Organisers");

            migrationBuilder.DropIndex(
                name: "IX_Organisers_OrganiserEmail",
                table: "Organisers");

            migrationBuilder.DropColumn(
                name: "Distance",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "EventType",
                table: "Events");

            migrationBuilder.RenameTable(
                name: "Organisers",
                newName: "Organiser");

            migrationBuilder.AlterColumn<string>(
                name: "CategoryName",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "OrganiserPassword",
                table: "Organiser",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "OrganiserName",
                table: "Organiser",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "OrganiserEmail",
                table: "Organiser",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Organiser",
                table: "Organiser",
                column: "OrganiserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Organiser_OrganiserId",
                table: "Events",
                column: "OrganiserId",
                principalTable: "Organiser",
                principalColumn: "OrganiserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
