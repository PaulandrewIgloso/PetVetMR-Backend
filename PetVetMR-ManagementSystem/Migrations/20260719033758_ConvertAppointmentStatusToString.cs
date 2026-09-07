using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetVetMR_ManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class ConvertAppointmentStatusToString : Migration
    {
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql(
				"ALTER TABLE [Appointments] DROP CONSTRAINT [CK__Appointme__Statu__6D0D32F4];");

			migrationBuilder.AlterColumn<string>(
				name: "Status",
				table: "Appointments",
				type: "nvarchar(50)",
				maxLength: 50,
				nullable: false,
				oldClrType: typeof(int),
				oldType: "int",
				oldMaxLength: 50);

			migrationBuilder.Sql(
				"ALTER TABLE [Appointments] ADD CONSTRAINT [CK__Appointme__Statu__6D0D32F4] " +
				"CHECK ([Status]='NoShow' OR [Status]='Cancelled' OR [Status]='Completed' OR [Status]='Scheduled');");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql(
				"ALTER TABLE [Appointments] DROP CONSTRAINT [CK__Appointme__Statu__6D0D32F4];");

			migrationBuilder.AlterColumn<int>(
				name: "Status",
				table: "Appointments",
				type: "int",
				maxLength: 50,
				nullable: false,
				oldClrType: typeof(string),
				oldType: "nvarchar(50)",
				oldMaxLength: 50);

			migrationBuilder.Sql(
				"ALTER TABLE [Appointments] ADD CONSTRAINT [CK__Appointme__Statu__6D0D32F4] " +
				"CHECK ([Status]='NoShow' OR [Status]='Cancelled' OR [Status]='Completed' OR [Status]='Scheduled');");
		}
	}
}
