using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuantityMeasurementRepositoryLayer.Migrations
{
    public partial class AddIdToQuantityMeasurements : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QuantityMeasurements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Operation = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FirstValue = table.Column<double>(type: "float", nullable: false),
                    FirstUnit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SecondValue = table.Column<double>(type: "float", nullable: false),
                    SecondUnit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Result = table.Column<double>(type: "float", nullable: false),
                    MeasurementType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuantityMeasurements", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "QuantityMeasurements");
        }
    }
}
