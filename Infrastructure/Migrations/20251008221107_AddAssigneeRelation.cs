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
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Users_UserEntityId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_UserEntityId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Assignee",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "UserEntityId",
                table: "Tasks");

            migrationBuilder.AddColumn<int>(
                name: "AssigneeId",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_AssigneeId",
                table: "Tasks",
                column: "AssigneeId");

            migrationBuilder.AddForeignKey(
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
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Users_AssigneeId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_AssigneeId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "AssigneeId",
                table: "Tasks");

            migrationBuilder.AddColumn<string>(
                name: "Assignee",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "UserEntityId",
                table: "Tasks",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_UserEntityId",
                table: "Tasks",
                column: "UserEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Users_UserEntityId",
                table: "Tasks",
                column: "UserEntityId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
