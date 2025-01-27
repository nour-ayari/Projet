using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Projet.Migrations
{
    /// <inheritdoc />
    public partial class mm5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
    name: "Interviews",
    columns: table => new
    {
        InterviewId = table.Column<int>(type: "int", nullable: false)
            .Annotation("SqlServer:Identity", "1, 1"),
        JobApplicationId = table.Column<int>(type: "int", nullable: false), // Make this non-nullable
        ScheduledDate = table.Column<DateTime>(type: "datetime2", nullable: false),
        Feedback = table.Column<string>(type: "nvarchar(max)", nullable: false),
        Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
    },
    constraints: table =>
    {
        table.PrimaryKey("PK_Interviews", x => x.InterviewId);
        table.ForeignKey(
            name: "FK_Interviews_JobApplications_JobApplicationId",
            column: x => x.JobApplicationId,
            principalTable: "JobApplications",
            principalColumn: "JobApplicationId",
            onDelete: ReferentialAction.Cascade); // Optional: Cascade on delete
    });

        }
    }
}