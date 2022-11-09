using JyskCashpoint.Models;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace JyskCashpoint.Migrations
{
    public partial class banknotes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Banknote",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Quantity = table.Column<int>(nullable: false),
                    Value = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Banknote", x => x.Id);
                });
            migrationBuilder.Sql("Insert INTO Banknote VALUES (5,200)");
            migrationBuilder.Sql("Insert INTO Banknote VALUES (5,100)");
            migrationBuilder.Sql("Insert INTO Banknote VALUES (5,50)");
            migrationBuilder.Sql("Insert INTO Banknote VALUES (5,20)");
            migrationBuilder.Sql("Insert INTO Banknote VALUES (5,10)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Banknote");
        }
    }
}
