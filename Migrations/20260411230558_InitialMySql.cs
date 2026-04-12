using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectManagement.Migrations
{
    /// <inheritdoc />
    public partial class InitialMySql : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TbUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PasswordHash = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Role = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Users__3214EC07E9B48031", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TbNotifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    Type = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Message = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsRead = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Notifica__3214EC07D6006DDB", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Notificat__UserI__4E88ABD4",
                        column: x => x.UserId,
                        principalTable: "TbUsers",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TbProjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Projects__3214EC077FDC379D", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Projects__Create__44FF419A",
                        column: x => x.CreatedBy,
                        principalTable: "TbUsers",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TbProjectMembers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    ProjectId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    Role = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    JoinedAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ProjectM__3214EC07F20E807A", x => x.Id);
                    table.ForeignKey(
                        name: "FK__ProjectMe__Proje__45F365D3",
                        column: x => x.ProjectId,
                        principalTable: "TbProjects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__ProjectMe__UserI__46E78A0C",
                        column: x => x.UserId,
                        principalTable: "TbUsers",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TbTasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ProjectId = table.Column<int>(type: "int", nullable: true),
                    Title = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AssignedTo = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    Priority = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DueDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Tasks__3214EC07B727A843", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Tasks__AssignedT__48CFD27E",
                        column: x => x.AssignedTo,
                        principalTable: "TbUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__Tasks__CreatedBy__49C3F6B7",
                        column: x => x.CreatedBy,
                        principalTable: "TbUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__Tasks__ProjectId__47DBAE45",
                        column: x => x.ProjectId,
                        principalTable: "TbProjects",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TbAttachments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    TaskId = table.Column<int>(type: "int", nullable: true),
                    FileUrl = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UploadedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Attachme__3214EC073AE3B018", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Attachmen__TaskI__4CA06362",
                        column: x => x.TaskId,
                        principalTable: "TbTasks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__Attachmen__Uploa__4D94879B",
                        column: x => x.UploadedBy,
                        principalTable: "TbUsers",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TbComments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    TaskId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    Content = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Comments__3214EC07279B244E", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Comments__TaskId__4AB81AF0",
                        column: x => x.TaskId,
                        principalTable: "TbTasks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__Comments__UserId__4BAC3F29",
                        column: x => x.UserId,
                        principalTable: "TbUsers",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TbTaskHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    TaskId = table.Column<int>(type: "int", nullable: true),
                    ChangedBy = table.Column<int>(type: "int", nullable: true),
                    FieldChanged = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OldValue = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NewValue = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ChangedAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TaskHist__3214EC075D06874D", x => x.Id);
                    table.ForeignKey(
                        name: "FK__TaskHisto__Chang__5070F446",
                        column: x => x.ChangedBy,
                        principalTable: "TbUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__TaskHisto__TaskI__4F7CD00D",
                        column: x => x.TaskId,
                        principalTable: "TbTasks",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_TbAttachments_TaskId",
                table: "TbAttachments",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_TbAttachments_UploadedBy",
                table: "TbAttachments",
                column: "UploadedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TbComments_TaskId",
                table: "TbComments",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_TbComments_UserId",
                table: "TbComments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TbNotifications_UserId",
                table: "TbNotifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TbProjectMembers_ProjectId",
                table: "TbProjectMembers",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TbProjectMembers_UserId",
                table: "TbProjectMembers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TbProjects_CreatedBy",
                table: "TbProjects",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TbTaskHistory_ChangedBy",
                table: "TbTaskHistory",
                column: "ChangedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TbTaskHistory_TaskId",
                table: "TbTaskHistory",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_TbTasks_AssignedTo",
                table: "TbTasks",
                column: "AssignedTo");

            migrationBuilder.CreateIndex(
                name: "IX_TbTasks_CreatedBy",
                table: "TbTasks",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TbTasks_ProjectId",
                table: "TbTasks",
                column: "ProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TbAttachments");

            migrationBuilder.DropTable(
                name: "TbComments");

            migrationBuilder.DropTable(
                name: "TbNotifications");

            migrationBuilder.DropTable(
                name: "TbProjectMembers");

            migrationBuilder.DropTable(
                name: "TbTaskHistory");

            migrationBuilder.DropTable(
                name: "TbTasks");

            migrationBuilder.DropTable(
                name: "TbProjects");

            migrationBuilder.DropTable(
                name: "TbUsers");
        }
    }
}
