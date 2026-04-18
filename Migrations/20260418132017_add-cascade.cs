using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectManagement.Migrations
{
    /// <inheritdoc />
    public partial class addcascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__TaskHisto__TaskI__4F7CD00D",
                table: "TbTaskHistory");

            migrationBuilder.AddForeignKey(
                name: "FK__TaskHisto__TaskI__4F7CD00D",
                table: "TbTaskHistory",
                column: "TaskId",
                principalTable: "TbTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__TaskHisto__TaskI__4F7CD00D",
                table: "TbTaskHistory");


            migrationBuilder.AddForeignKey(
                name: "FK__TaskHisto__TaskI__4F7CD00D",
                table: "TbTaskHistory",
                column: "TaskId",
                principalTable: "TbTasks",
                principalColumn: "Id");
        }
    }
}
