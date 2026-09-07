using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetVetMR_ManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddFilteredUniqueIndexOnMicrochipID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pets_Users_OwnerUserID",
                table: "Pets");

            migrationBuilder.CreateIndex(
                name: "IX_Pets_MicrochipID",
                table: "Pets",
                column: "MicrochipID",
                unique: true,
                filter: "[MicrochipID] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Pets_Users_OwnerUserID",
                table: "Pets",
                column: "OwnerUserID",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pets_Users_OwnerUserID",
                table: "Pets");

            migrationBuilder.DropIndex(
                name: "IX_Pets_MicrochipID",
                table: "Pets");

            migrationBuilder.AddForeignKey(
                name: "FK_Pets_Users_OwnerUserID",
                table: "Pets",
                column: "OwnerUserID",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
