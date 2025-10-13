using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAssigneeRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            ArgumentNullException.ThrowIfNull(migrationBuilder);

            _ = migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Users_UserEntityId",
                table: "Tasks");

            _ = migrationBuilder.DropIndex(
                name: "IX_Tasks_UserEntityId",
                table: "Tasks");

            _ = migrationBuilder.DropColumn(
                name: "Assignee",
                table: "Tasks");

            _ = migrationBuilder.DropColumn(
                name: "UserEntityId",
                table: "Tasks");

            _ = migrationBuilder.AddColumn<int>(
                name: "AssigneeId",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            _ = migrationBuilder.CreateIndex(
                name: "IX_Tasks_AssigneeId",
                table: "Tasks",
                column: "AssigneeId");

            _ = migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Users_AssigneeId",
                table: "Tasks",
                column: "AssigneeId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            ArgumentNullException.ThrowIfNull(migrationBuilder);

            _ = migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Users_AssigneeId",
                table: "Tasks");

            _ = migrationBuilder.DropIndex(
                name: "IX_Tasks_AssigneeId",
                table: "Tasks");

            _ = migrationBuilder.DropColumn(
                name: "AssigneeId",
                table: "Tasks");

            _ = migrationBuilder.AddColumn<string>(
                name: "Assignee",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            _ = migrationBuilder.AddColumn<int>(
                name: "UserEntityId",
                table: "Tasks",
                type: "int",
                nullable: true);

            _ = migrationBuilder.CreateIndex(
                name: "IX_Tasks_UserEntityId",
                table: "Tasks",
                column: "UserEntityId");

            _ = migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Users_UserEntityId",
                table: "Tasks",
                column: "UserEntityId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
