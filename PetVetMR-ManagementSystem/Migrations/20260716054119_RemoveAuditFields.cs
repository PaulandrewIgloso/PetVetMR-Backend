using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetVetMR_ManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAuditFields : Migration
    {
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK__MedicalRe__Creat__5CD6CB2B",
				table: "MedicalRecords");

			migrationBuilder.DropForeignKey(
				name: "FK__MedicalRe__Updat__5EBF139D",
				table: "MedicalRecords");

			migrationBuilder.DropForeignKey(
				name: "FK__Pets__CreatedByU__571DF1D5",
				table: "Pets");

			migrationBuilder.DropColumn(
				name: "LastLogin",
				table: "Users");

			migrationBuilder.DropColumn(
				name: "CreatedByUserID",
				table: "Pets");

			migrationBuilder.DropColumn(
				name: "CreatedByUserID",
				table: "MedicalRecords");

			migrationBuilder.DropColumn(
				name: "UpdatedByUserID",
				table: "MedicalRecords");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastLogin",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserID",
                table: "Pets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserID",
                table: "MedicalRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUserID",
                table: "MedicalRecords",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pets_CreatedByUserID",
                table: "Pets",
                column: "CreatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecords_CreatedByUserID",
                table: "MedicalRecords",
                column: "CreatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecords_UpdatedByUserID",
                table: "MedicalRecords",
                column: "UpdatedByUserID");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalRecords_Users_CreatedByUserID",
                table: "MedicalRecords",
                column: "CreatedByUserID",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalRecords_Users_UpdatedByUserID",
                table: "MedicalRecords",
                column: "UpdatedByUserID",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Pets_Users_CreatedByUserID",
                table: "Pets",
                column: "CreatedByUserID",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
