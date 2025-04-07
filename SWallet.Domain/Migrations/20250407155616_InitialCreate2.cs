using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SWallet.Domain.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbl_challenge_transaction_tbl_student_challenge_challenge_id",
                table: "challenge_transaction");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tbl_student_challenge",
                table: "student_challenge");

            migrationBuilder.DropColumn(
                name: "state",
                table: "wallet");

            migrationBuilder.DropColumn(
                name: "id",
                table: "student_challenge");

            migrationBuilder.DropColumn(
                name: "state",
                table: "student_challenge");

            migrationBuilder.DropColumn(
                name: "state",
                table: "campaign_store");

            migrationBuilder.DropColumn(
                name: "state",
                table: "activity_transaction");

            migrationBuilder.AlterColumn<string>(
                name: "student_id",
                table: "wallet",
                type: "char(26)",
                unicode: false,
                fixedLength: true,
                maxLength: 26,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(26)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 26);

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "wallet",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "campaign_id",
                table: "wallet",
                type: "char(26)",
                unicode: false,
                fixedLength: true,
                maxLength: 26,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(26)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 26);

            migrationBuilder.AlterColumn<string>(
                name: "brand_id",
                table: "wallet",
                type: "char(26)",
                unicode: false,
                fixedLength: true,
                maxLength: 26,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(26)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 26);

            migrationBuilder.AddColumn<string>(
                name: "campus_id",
                table: "wallet",
                type: "char(26)",
                unicode: false,
                fixedLength: true,
                maxLength: 26,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "lecturer_id",
                table: "wallet",
                type: "char(26)",
                unicode: false,
                fixedLength: true,
                maxLength: 26,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "voucher_type",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "voucher",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "condition",
                table: "voucher",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "student_challenge",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<DateTime>(
                name: "dateCompleted",
                table: "student_challenge",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "address",
                table: "student",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "student_email",
                table: "student",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "store",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "address",
                table: "store",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "state",
                table: "challenge_transaction",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "challenge_id",
                table: "challenge_transaction",
                type: "char(26)",
                unicode: false,
                fixedLength: true,
                maxLength: 26,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(26)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 26);

            migrationBuilder.AddColumn<string>(
                name: "student_id",
                table: "challenge_transaction",
                type: "char(26)",
                unicode: false,
                fixedLength: true,
                maxLength: 26,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "image",
                table: "challenge",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "file_name",
                table: "challenge",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "challenge",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "account_id",
                table: "campus",
                type: "char(26)",
                unicode: false,
                fixedLength: true,
                maxLength: 26,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "wallet_id",
                table: "activity_transaction",
                type: "char(26)",
                unicode: false,
                fixedLength: true,
                maxLength: 26,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(26)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 26);

            migrationBuilder.AlterColumn<string>(
                name: "store_id",
                table: "activity",
                type: "char(26)",
                unicode: false,
                fixedLength: true,
                maxLength: 26,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(26)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 26);

            migrationBuilder.AlterColumn<string>(
                name: "phone",
                table: "account",
                type: "char(10)",
                unicode: false,
                fixedLength: true,
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(20)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "account",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_student_challenge",
                table: "student_challenge",
                columns: new[] { "challenge_id", "student_id" });

            migrationBuilder.CreateTable(
                name: "campus_lecturer",
                columns: table => new
                {
                    id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    campus_id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: true),
                    lecturer_id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: true),
                    status = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_campus_lecturer", x => x.id);
                    table.ForeignKey(
                        name: "FK_campus_lecturer_campus",
                        column: x => x.campus_id,
                        principalTable: "campus",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_campus_lecturer_lecturer",
                        column: x => x.lecturer_id,
                        principalTable: "lecturer",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "luckyPrize",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    prize_name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    probability = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    quantity = table.Column<int>(type: "int", nullable: true),
                    status = table.Column<bool>(type: "bit", nullable: true),
                    value = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_luckyPrize", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "point_package",
                columns: table => new
                {
                    id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    package_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    point = table.Column<int>(type: "int", nullable: true),
                    price = table.Column<decimal>(type: "decimal(38,2)", nullable: true),
                    date_created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    date_updated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    status = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Point_package", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "qr_code_history",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    lectureId = table.Column<string>(type: "varchar(26)", unicode: false, maxLength: 26, nullable: false),
                    points = table.Column<int>(type: "int", nullable: false),
                    startOnTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    expirationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    qrCodeData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    qrCodeImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_qr_code_history", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "qrcode_usage",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    qrcode_json = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    student_id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: true),
                    used_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_qrcode_usage", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_wallet_campus_id",
                table: "wallet",
                column: "campus_id");

            migrationBuilder.CreateIndex(
                name: "IX_wallet_lecturer_id",
                table: "wallet",
                column: "lecturer_id");

            migrationBuilder.CreateIndex(
                name: "IX_challenge_transaction_challenge_id_student_id",
                table: "challenge_transaction",
                columns: new[] { "challenge_id", "student_id" });

            migrationBuilder.CreateIndex(
                name: "IX_campus_account_id",
                table: "campus",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "IX_campus_lecturer_campus_id",
                table: "campus_lecturer",
                column: "campus_id");

            migrationBuilder.CreateIndex(
                name: "IX_campus_lecturer_lecturer_id",
                table: "campus_lecturer",
                column: "lecturer_id");

            migrationBuilder.AddForeignKey(
                name: "FK_campus_account",
                table: "campus",
                column: "account_id",
                principalTable: "account",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_challenge_transaction_student_challenge",
                table: "challenge_transaction",
                columns: new[] { "challenge_id", "student_id" },
                principalTable: "student_challenge",
                principalColumns: new[] { "challenge_id", "student_id" });

            migrationBuilder.AddForeignKey(
                name: "FK_wallet_campus",
                table: "wallet",
                column: "campus_id",
                principalTable: "campus",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_wallet_lecturer",
                table: "wallet",
                column: "lecturer_id",
                principalTable: "lecturer",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_campus_account",
                table: "campus");

            migrationBuilder.DropForeignKey(
                name: "FK_challenge_transaction_student_challenge",
                table: "challenge_transaction");

            migrationBuilder.DropForeignKey(
                name: "FK_wallet_campus",
                table: "wallet");

            migrationBuilder.DropForeignKey(
                name: "FK_wallet_lecturer",
                table: "wallet");

            migrationBuilder.DropTable(
                name: "campus_lecturer");

            migrationBuilder.DropTable(
                name: "luckyPrize");

            migrationBuilder.DropTable(
                name: "point_package");

            migrationBuilder.DropTable(
                name: "qr_code_history");

            migrationBuilder.DropTable(
                name: "qrcode_usage");

            migrationBuilder.DropIndex(
                name: "IX_wallet_campus_id",
                table: "wallet");

            migrationBuilder.DropIndex(
                name: "IX_wallet_lecturer_id",
                table: "wallet");

            migrationBuilder.DropPrimaryKey(
                name: "PK_student_challenge",
                table: "student_challenge");

            migrationBuilder.DropIndex(
                name: "IX_challenge_transaction_challenge_id_student_id",
                table: "challenge_transaction");

            migrationBuilder.DropIndex(
                name: "IX_campus_account_id",
                table: "campus");

            migrationBuilder.DropColumn(
                name: "campus_id",
                table: "wallet");

            migrationBuilder.DropColumn(
                name: "lecturer_id",
                table: "wallet");

            migrationBuilder.DropColumn(
                name: "dateCompleted",
                table: "student_challenge");

            migrationBuilder.DropColumn(
                name: "student_email",
                table: "student");

            migrationBuilder.DropColumn(
                name: "student_id",
                table: "challenge_transaction");

            migrationBuilder.DropColumn(
                name: "account_id",
                table: "campus");

            migrationBuilder.AlterColumn<string>(
                name: "student_id",
                table: "wallet",
                type: "char(26)",
                unicode: false,
                fixedLength: true,
                maxLength: 26,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "char(26)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 26,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "wallet",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "campaign_id",
                table: "wallet",
                type: "char(26)",
                unicode: false,
                fixedLength: true,
                maxLength: 26,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "char(26)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 26,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "brand_id",
                table: "wallet",
                type: "char(26)",
                unicode: false,
                fixedLength: true,
                maxLength: 26,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "char(26)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 26,
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "state",
                table: "wallet",
                type: "bit",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "voucher_type",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "voucher",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "condition",
                table: "voucher",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "student_challenge",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "id",
                table: "student_challenge",
                type: "char(26)",
                unicode: false,
                fixedLength: true,
                maxLength: 26,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "state",
                table: "student_challenge",
                type: "bit",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "address",
                table: "student",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "store",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "address",
                table: "store",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<bool>(
                name: "state",
                table: "challenge_transaction",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "challenge_id",
                table: "challenge_transaction",
                type: "char(26)",
                unicode: false,
                fixedLength: true,
                maxLength: 26,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "char(26)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 26,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "image",
                table: "challenge",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "file_name",
                table: "challenge",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "challenge",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "state",
                table: "campaign_store",
                type: "bit",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "wallet_id",
                table: "activity_transaction",
                type: "char(26)",
                unicode: false,
                fixedLength: true,
                maxLength: 26,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "char(26)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 26,
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "state",
                table: "activity_transaction",
                type: "bit",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "store_id",
                table: "activity",
                type: "char(26)",
                unicode: false,
                fixedLength: true,
                maxLength: 26,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "char(26)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 26,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "phone",
                table: "account",
                type: "char(20)",
                unicode: false,
                fixedLength: true,
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(10)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "account",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_tbl_student_challenge",
                table: "student_challenge",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_challenge_transaction_tbl_student_challenge_challenge_id",
                table: "challenge_transaction",
                column: "challenge_id",
                principalTable: "student_challenge",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
