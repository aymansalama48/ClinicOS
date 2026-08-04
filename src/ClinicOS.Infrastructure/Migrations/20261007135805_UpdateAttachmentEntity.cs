using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicOS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAttachmentEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SpecializationSchedules_Specialization_Day_Period",
                table: "SpecializationSchedules");

            migrationBuilder.DropIndex(
                name: "IX_Specializations_Name_IsDeleted",
                table: "Specializations");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "SpecializationSchedules",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "SpecializationSchedules",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "SpecializationSchedules",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "SpecializationSchedules",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "SpecializationSchedules",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "SpecializationSchedules",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "SpecializationSchedules",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "SpecializationSchedules",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "IconAttachmentId",
                table: "Specializations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Specializations",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AlterColumn<string>(
                name: "EntityType",
                table: "Attachments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<Guid>(
                name: "EntityId",
                table: "Attachments",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<string>(
                name: "StoredName",
                table: "Attachments",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_SpecializationSchedules_Specialization_Day_Period",
                table: "SpecializationSchedules",
                columns: new[] { "SpecializationId", "DayOfWeek", "Period" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Specializations_IconAttachmentId",
                table: "Specializations",
                column: "IconAttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Specializations_Name",
                table: "Specializations",
                column: "Name",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.AddForeignKey(
                name: "FK_Specializations_Attachments_IconAttachmentId",
                table: "Specializations",
                column: "IconAttachmentId",
                principalTable: "Attachments",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Specializations_Attachments_IconAttachmentId",
                table: "Specializations");

            migrationBuilder.DropIndex(
                name: "IX_SpecializationSchedules_Specialization_Day_Period",
                table: "SpecializationSchedules");

            migrationBuilder.DropIndex(
                name: "IX_Specializations_IconAttachmentId",
                table: "Specializations");

            migrationBuilder.DropIndex(
                name: "IX_Specializations_Name",
                table: "Specializations");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "SpecializationSchedules");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "SpecializationSchedules");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "SpecializationSchedules");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "SpecializationSchedules");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "SpecializationSchedules");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "SpecializationSchedules");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "SpecializationSchedules");

            migrationBuilder.DropColumn(
                name: "IconAttachmentId",
                table: "Specializations");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Specializations");

            migrationBuilder.DropColumn(
                name: "StoredName",
                table: "Attachments");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "SpecializationSchedules",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<string>(
                name: "EntityType",
                table: "Attachments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "EntityId",
                table: "Attachments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SpecializationSchedules_Specialization_Day_Period",
                table: "SpecializationSchedules",
                columns: new[] { "SpecializationId", "DayOfWeek", "Period" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Specializations_Name_IsDeleted",
                table: "Specializations",
                columns: new[] { "Name", "IsDeleted" },
                unique: true,
                filter: "IsDeleted = 0");
        }
    }
}
