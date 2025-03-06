using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SWallet.Domain.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "account",
                columns: table => new
                {
                    id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    role = table.Column<int>(type: "int", nullable: true),
                    user_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    phone = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: true),
                    email = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: true),
                    avatar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    file_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_verify = table.Column<bool>(type: "bit", nullable: true),
                    date_created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    date_updated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    date_verified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    state = table.Column<bool>(type: "bit", nullable: true),
                    status = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_account", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "area",
                columns: table => new
                {
                    id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    area_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    image = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    file_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    date_created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    date_updated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    state = table.Column<bool>(type: "bit", nullable: true),
                    status = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_area", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "campaign_type",
                columns: table => new
                {
                    id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    type_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    image = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    file_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    date_created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    date_updated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    state = table.Column<bool>(type: "bit", nullable: true),
                    status = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_campaign_type", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "category",
                columns: table => new
                {
                    id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    category_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    image = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    file_name = table.Column<string>(type: "text", nullable: false),
                    date_created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    date_updated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    description = table.Column<string>(type: "text", nullable: false),
                    state = table.Column<bool>(type: "bit", nullable: true),
                    status = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_category", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "challenge",
                columns: table => new
                {
                    id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    type = table.Column<int>(type: "int", nullable: true),
                    challenge_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    amount = table.Column<decimal>(type: "decimal(38,2)", nullable: true),
                    condition = table.Column<decimal>(type: "decimal(38,2)", nullable: true),
                    image = table.Column<string>(type: "text", nullable: false),
                    file_name = table.Column<string>(type: "text", nullable: false),
                    date_created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    date_updated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    description = table.Column<string>(type: "text", nullable: false),
                    state = table.Column<bool>(type: "bit", nullable: true),
                    status = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_challenge", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "station",
                columns: table => new
                {
                    id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    station_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    address = table.Column<string>(type: "text", nullable: false),
                    image = table.Column<string>(type: "text", nullable: false),
                    file_name = table.Column<string>(type: "text", nullable: false),
                    opening_hours = table.Column<TimeOnly>(type: "time", nullable: true),
                    closing_hours = table.Column<TimeOnly>(type: "time", nullable: true),
                    phone = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                    email = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                    date_created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    date_updated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    description = table.Column<string>(type: "text", nullable: false),
                    state = table.Column<int>(type: "int", nullable: true),
                    status = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_station", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "voucher_type",
                columns: table => new
                {
                    id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    type_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    image = table.Column<string>(type: "text", nullable: false),
                    file_name = table.Column<string>(type: "text", nullable: false),
                    date_created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    date_updated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    description = table.Column<string>(type: "text", nullable: false),
                    state = table.Column<bool>(type: "bit", nullable: true),
                    status = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_voucher_type", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "admin",
                columns: table => new
                {
                    id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    account_id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    full_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    date_created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    date_updated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    state = table.Column<bool>(type: "bit", nullable: true),
                    status = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_admin", x => x.id);
                    table.ForeignKey(
                        name: "FK_tbl_admin_tbl_account_account_id",
                        column: x => x.account_id,
                        principalTable: "account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "brand",
                columns: table => new
                {
                    id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    account_id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    brand_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    acronym = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    cover_photo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    cover_file_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    link = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    opening_hours = table.Column<TimeOnly>(type: "time", nullable: true),
                    closing_hours = table.Column<TimeOnly>(type: "time", nullable: true),
                    total_income = table.Column<decimal>(type: "decimal(38,2)", nullable: true),
                    total_spending = table.Column<decimal>(type: "decimal(38,2)", nullable: true),
                    date_created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    date_updated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    state = table.Column<bool>(type: "bit", nullable: true),
                    status = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_brand", x => x.id);
                    table.ForeignKey(
                        name: "FK_tbl_brand_tbl_account_account_id",
                        column: x => x.account_id,
                        principalTable: "account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "lecturer",
                columns: table => new
                {
                    id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    account_id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    full_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    date_created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    date_updated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    state = table.Column<bool>(type: "bit", nullable: true),
                    status = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_staff", x => x.id);
                    table.ForeignKey(
                        name: "FK_tbl_staff_tbl_account_account_id",
                        column: x => x.account_id,
                        principalTable: "account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "campus",
                columns: table => new
                {
                    id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    area_id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    campus_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    phone = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: true),
                    email = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: true),
                    link_website = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    file_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    date_created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    date_updated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    state = table.Column<bool>(type: "bit", nullable: true),
                    status = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_campus", x => x.id);
                    table.ForeignKey(
                        name: "FK_tbl_campus_tbl_area_area_id",
                        column: x => x.area_id,
                        principalTable: "area",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "product",
                columns: table => new
                {
                    id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    category_id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    product_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    price = table.Column<decimal>(type: "decimal(38,2)", nullable: true),
                    weight = table.Column<decimal>(type: "decimal(38,2)", nullable: true),
                    quantity = table.Column<int>(type: "int", nullable: true),
                    date_created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    date_updated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    description = table.Column<string>(type: "text", nullable: false),
                    state = table.Column<bool>(type: "bit", nullable: true),
                    status = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_product", x => x.id);
                    table.ForeignKey(
                        name: "FK_tbl_product_tbl_category_category_id",
                        column: x => x.category_id,
                        principalTable: "category",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "campaign",
                columns: table => new
                {
                    id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    brand_id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    type_id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    campaign_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    image = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    image_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    file = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    file_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    condition = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    link = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    start_on = table.Column<DateOnly>(type: "date", nullable: true),
                    end_on = table.Column<DateOnly>(type: "date", nullable: true),
                    duration = table.Column<int>(type: "int", nullable: true),
                    total_income = table.Column<decimal>(type: "decimal(38,2)", nullable: true),
                    total_spending = table.Column<decimal>(type: "decimal(38,2)", nullable: true),
                    date_created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    date_updated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    status = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_campaign", x => x.id);
                    table.ForeignKey(
                        name: "FK_tbl_campaign_tbl_brand_brand_id",
                        column: x => x.brand_id,
                        principalTable: "brand",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbl_campaign_tbl_campaign_type_type_id",
                        column: x => x.type_id,
                        principalTable: "campaign_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "request",
                columns: table => new
                {
                    id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    brand_id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    admin_id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    amount = table.Column<decimal>(type: "decimal(38,2)", nullable: true),
                    date_created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    date_updated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    description = table.Column<string>(type: "text", nullable: false),
                    state = table.Column<bool>(type: "bit", nullable: true),
                    status = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_request", x => x.id);
                    table.ForeignKey(
                        name: "FK_tbl_request_tbl_admin_admin_id",
                        column: x => x.admin_id,
                        principalTable: "admin",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbl_request_tbl_brand_brand_id",
                        column: x => x.brand_id,
                        principalTable: "brand",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "store",
                columns: table => new
                {
                    id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    brand_id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    area_id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    account_id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    store_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    address = table.Column<string>(type: "text", nullable: false),
                    opening_hours = table.Column<TimeOnly>(type: "time", nullable: true),
                    closing_hours = table.Column<TimeOnly>(type: "time", nullable: true),
                    file = table.Column<string>(type: "text", nullable: false),
                    file_name = table.Column<string>(type: "text", nullable: false),
                    date_created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    date_updated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    description = table.Column<string>(type: "text", nullable: false),
                    state = table.Column<bool>(type: "bit", nullable: true),
                    status = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_store", x => x.id);
                    table.ForeignKey(
                        name: "FK_tbl_store_tbl_account_account_id",
                        column: x => x.account_id,
                        principalTable: "account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbl_store_tbl_area_area_id",
                        column: x => x.area_id,
                        principalTable: "area",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_tbl_store_tbl_brand_brand_id",
                        column: x => x.brand_id,
                        principalTable: "brand",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "voucher",
                columns: table => new
                {
                    id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    brand_id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    type_id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    voucher_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    price = table.Column<decimal>(type: "decimal(38,2)", nullable: true),
                    rate = table.Column<decimal>(type: "decimal(38,2)", nullable: true),
                    condition = table.Column<string>(type: "text", nullable: false),
                    image = table.Column<string>(type: "text", nullable: false),
                    image_name = table.Column<string>(type: "text", nullable: false),
                    file = table.Column<string>(type: "text", nullable: false),
                    file_name = table.Column<string>(type: "text", nullable: false),
                    date_created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    date_updated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    description = table.Column<string>(type: "text", nullable: false),
                    state = table.Column<bool>(type: "bit", nullable: true),
                    status = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_voucher", x => x.id);
                    table.ForeignKey(
                        name: "FK_tbl_voucher_tbl_brand_brand_id",
                        column: x => x.brand_id,
                        principalTable: "brand",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbl_voucher_tbl_voucher_type_type_id",
                        column: x => x.type_id,
                        principalTable: "voucher_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "student",
                columns: table => new
                {
                    id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    campus_id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: true),
                    account_id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    student_card_front = table.Column<string>(type: "text", nullable: true),
                    file_name_front = table.Column<string>(type: "text", nullable: true),
                    student_card_back = table.Column<string>(type: "text", nullable: true),
                    file_name_back = table.Column<string>(type: "text", nullable: true),
                    full_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    code = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    gender = table.Column<int>(type: "int", nullable: true),
                    date_of_birth = table.Column<DateOnly>(type: "date", nullable: true),
                    address = table.Column<string>(type: "text", nullable: false),
                    total_income = table.Column<decimal>(type: "decimal(38,2)", nullable: true),
                    total_spending = table.Column<decimal>(type: "decimal(38,2)", nullable: true),
                    date_created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    date_updated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    state = table.Column<int>(type: "int", nullable: true),
                    status = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_student", x => x.id);
                    table.ForeignKey(
                        name: "FK_tbl_student_tbl_account_account_id",
                        column: x => x.account_id,
                        principalTable: "account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbl_student_tbl_campus_campus_id",
                        column: x => x.campus_id,
                        principalTable: "campus",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "campaign_campus",
                columns: table => new
                {
                    id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    campaign_id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    campus_id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    state = table.Column<bool>(type: "bit", nullable: true),
                    status = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_campaign_campus", x => x.id);
                    table.ForeignKey(
                        name: "FK_tbl_campaign_campus_tbl_campaign_campaign_id",
                        column: x => x.campaign_id,
                        principalTable: "campaign",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbl_campaign_campus_tbl_campus_campus_id",
                        column: x => x.campus_id,
                        principalTable: "campus",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "campaign_store",
                columns: table => new
                {
                    id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    campaign_id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    store_id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    state = table.Column<bool>(type: "bit", nullable: true),
                    status = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_campaign_store", x => x.id);
                    table.ForeignKey(
                        name: "FK_tbl_campaign_store_tbl_campaign_campaign_id",
                        column: x => x.campaign_id,
                        principalTable: "campaign",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbl_campaign_store_tbl_store_store_id",
                        column: x => x.store_id,
                        principalTable: "store",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "campaign_detail",
                columns: table => new
                {
                    id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    voucher_id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    campaign_id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    price = table.Column<decimal>(type: "decimal(38,2)", nullable: true),
                    rate = table.Column<decimal>(type: "decimal(38,2)", nullable: true),
                    quantity = table.Column<int>(type: "int", nullable: true),
                    from_index = table.Column<int>(type: "int", nullable: true),
                    to_index = table.Column<int>(type: "int", nullable: true),
                    date_created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    date_updated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    state = table.Column<bool>(type: "bit", nullable: true),
                    status = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_campaign_detail", x => x.id);
                    table.ForeignKey(
                        name: "FK_tbl_campaign_detail_tbl_campaign_campaign_id",
                        column: x => x.campaign_id,
                        principalTable: "campaign",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbl_campaign_detail_tbl_voucher_voucher_id",
                        column: x => x.voucher_id,
                        principalTable: "voucher",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "invitation",
                columns: table => new
                {
                    id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    inviter_id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    invitee_id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    date_created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    description = table.Column<string>(type: "text", nullable: false),
                    state = table.Column<bool>(type: "bit", nullable: true),
                    status = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_invitation", x => x.id);
                    table.ForeignKey(
                        name: "FK_tbl_invitation_tbl_student_invitee_id",
                        column: x => x.invitee_id,
                        principalTable: "student",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbl_invitation_tbl_student_inviter_id",
                        column: x => x.inviter_id,
                        principalTable: "student",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "order",
                columns: table => new
                {
                    id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    student_id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    station_id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    amount = table.Column<decimal>(type: "decimal(38,2)", nullable: true),
                    date_created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    description = table.Column<string>(type: "text", nullable: false),
                    state = table.Column<bool>(type: "bit", nullable: true),
                    status = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_order", x => x.id);
                    table.ForeignKey(
                        name: "FK_tbl_order_tbl_station_station_id",
                        column: x => x.station_id,
                        principalTable: "station",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbl_order_tbl_student_student_id",
                        column: x => x.student_id,
                        principalTable: "student",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "reward",
                columns: table => new
                {
                    id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    brand_id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    student_id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    store_id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    amount = table.Column<decimal>(type: "decimal(38,2)", nullable: true),
                    date_created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    date_updated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    state = table.Column<bool>(type: "bit", nullable: true),
                    status = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_bonus", x => x.id);
                    table.ForeignKey(
                        name: "FK_tbl_bonus_tbl_brand_brand_id",
                        column: x => x.brand_id,
                        principalTable: "brand",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbl_bonus_tbl_store_store_id",
                        column: x => x.store_id,
                        principalTable: "store",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_tbl_bonus_tbl_student_student_id",
                        column: x => x.student_id,
                        principalTable: "student",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "student_challenge",
                columns: table => new
                {
                    id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    challenge_id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    student_id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    amount = table.Column<decimal>(type: "decimal(38,2)", nullable: true),
                    current = table.Column<decimal>(type: "decimal(38,2)", nullable: true),
                    condition = table.Column<decimal>(type: "decimal(38,2)", nullable: true),
                    is_completed = table.Column<bool>(type: "bit", nullable: true),
                    date_created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    date_updated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    description = table.Column<string>(type: "text", nullable: false),
                    state = table.Column<bool>(type: "bit", nullable: true),
                    status = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_student_challenge", x => x.id);
                    table.ForeignKey(
                        name: "FK_tbl_student_challenge_tbl_challenge_challenge_id",
                        column: x => x.challenge_id,
                        principalTable: "challenge",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbl_student_challenge_tbl_student_student_id",
                        column: x => x.student_id,
                        principalTable: "student",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "wallet",
                columns: table => new
                {
                    id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    campaign_id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    student_id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    brand_id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    type = table.Column<int>(type: "int", nullable: true),
                    balance = table.Column<decimal>(type: "decimal(38,2)", nullable: true),
                    date_created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    date_updated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    description = table.Column<string>(type: "text", nullable: false),
                    state = table.Column<bool>(type: "bit", nullable: true),
                    status = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_wallet", x => x.id);
                    table.ForeignKey(
                        name: "FK_tbl_wallet_tbl_brand_brand_id",
                        column: x => x.brand_id,
                        principalTable: "brand",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbl_wallet_tbl_campaign_campaign_id",
                        column: x => x.campaign_id,
                        principalTable: "campaign",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_tbl_wallet_tbl_student_student_id",
                        column: x => x.student_id,
                        principalTable: "student",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "voucher_item",
                columns: table => new
                {
                    id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    voucher_id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    campaign_detail_id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    voucher_code = table.Column<string>(type: "text", nullable: false),
                    index = table.Column<int>(type: "int", nullable: true),
                    is_locked = table.Column<bool>(type: "bit", nullable: true),
                    is_bought = table.Column<bool>(type: "bit", nullable: true),
                    is_used = table.Column<bool>(type: "bit", nullable: true),
                    valid_on = table.Column<DateOnly>(type: "date", nullable: true),
                    expire_on = table.Column<DateOnly>(type: "date", nullable: true),
                    date_created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    date_issued = table.Column<DateTime>(type: "datetime2", nullable: true),
                    state = table.Column<bool>(type: "bit", nullable: true),
                    status = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_voucher_item", x => x.id);
                    table.ForeignKey(
                        name: "FK_tbl_voucher_item_tbl_campaign_detail_campaign_detail_id",
                        column: x => x.campaign_detail_id,
                        principalTable: "campaign_detail",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbl_voucher_item_tbl_voucher_voucher_id",
                        column: x => x.voucher_id,
                        principalTable: "voucher",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "order_detail",
                columns: table => new
                {
                    id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    product_id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    order_id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    price = table.Column<decimal>(type: "decimal(38,2)", nullable: true),
                    quantity = table.Column<int>(type: "int", nullable: true),
                    amount = table.Column<decimal>(type: "decimal(38,2)", nullable: true),
                    state = table.Column<bool>(type: "bit", nullable: true),
                    status = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_order_detail", x => x.id);
                    table.ForeignKey(
                        name: "FK_tbl_order_detail_tbl_order_order_id",
                        column: x => x.order_id,
                        principalTable: "order",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbl_order_detail_tbl_product_product_id",
                        column: x => x.product_id,
                        principalTable: "product",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "order_state",
                columns: table => new
                {
                    id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    order_id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    state = table.Column<int>(type: "int", nullable: true),
                    date_created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    description = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_order_state", x => x.id);
                    table.ForeignKey(
                        name: "FK_tbl_order_state_tbl_order_order_id",
                        column: x => x.order_id,
                        principalTable: "order",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "campaign_transaction",
                columns: table => new
                {
                    id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    campaign_id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    wallet_id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    amount = table.Column<decimal>(type: "decimal(38,2)", nullable: true),
                    rate = table.Column<decimal>(type: "decimal(38,2)", nullable: true),
                    date_created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    state = table.Column<bool>(type: "bit", nullable: true),
                    status = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_campaign_transaction", x => x.id);
                    table.ForeignKey(
                        name: "FK_tbl_campaign_transaction_tbl_campaign_campaign_id",
                        column: x => x.campaign_id,
                        principalTable: "campaign",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbl_campaign_transaction_tbl_wallet_wallet_id",
                        column: x => x.wallet_id,
                        principalTable: "wallet",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "challenge_transaction",
                columns: table => new
                {
                    id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    wallet_id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    challenge_id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    amount = table.Column<decimal>(type: "decimal(38,2)", nullable: true),
                    rate = table.Column<decimal>(type: "decimal(38,2)", nullable: true),
                    date_created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    description = table.Column<string>(type: "text", nullable: false),
                    state = table.Column<bool>(type: "bit", nullable: true),
                    status = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_challenge_transaction", x => x.id);
                    table.ForeignKey(
                        name: "FK_tbl_challenge_transaction_tbl_student_challenge_challenge_id",
                        column: x => x.challenge_id,
                        principalTable: "student_challenge",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbl_challenge_transaction_tbl_wallet_wallet_id",
                        column: x => x.wallet_id,
                        principalTable: "wallet",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "order_transaction",
                columns: table => new
                {
                    id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    order_id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    wallet_id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    amount = table.Column<decimal>(type: "decimal(38,2)", nullable: true),
                    rate = table.Column<decimal>(type: "decimal(38,2)", nullable: true),
                    description = table.Column<string>(type: "text", nullable: false),
                    state = table.Column<bool>(type: "bit", nullable: true),
                    status = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_order_transaction", x => x.id);
                    table.ForeignKey(
                        name: "FK_tbl_order_transaction_tbl_order_order_id",
                        column: x => x.order_id,
                        principalTable: "order",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbl_order_transaction_tbl_wallet_wallet_id",
                        column: x => x.wallet_id,
                        principalTable: "wallet",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "request_transaction",
                columns: table => new
                {
                    id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    wallet_id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    request_id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    amount = table.Column<decimal>(type: "decimal(38,2)", nullable: true),
                    rate = table.Column<decimal>(type: "decimal(38,2)", nullable: true),
                    description = table.Column<string>(type: "text", nullable: false),
                    state = table.Column<bool>(type: "bit", nullable: true),
                    status = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_request_transaction", x => x.id);
                    table.ForeignKey(
                        name: "FK_tbl_request_transaction_tbl_request_request_id",
                        column: x => x.request_id,
                        principalTable: "request",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbl_request_transaction_tbl_wallet_wallet_id",
                        column: x => x.wallet_id,
                        principalTable: "wallet",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "reward_transaction",
                columns: table => new
                {
                    id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    wallet_id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    bonus_id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    amount = table.Column<decimal>(type: "decimal(38,2)", nullable: true),
                    rate = table.Column<decimal>(type: "decimal(38,2)", nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    state = table.Column<bool>(type: "bit", nullable: true),
                    status = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_bonus_transaction", x => x.id);
                    table.ForeignKey(
                        name: "FK_tbl_bonus_transaction_tbl_bonus_bonus_id",
                        column: x => x.bonus_id,
                        principalTable: "reward",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbl_bonus_transaction_tbl_wallet_wallet_id",
                        column: x => x.wallet_id,
                        principalTable: "wallet",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "activity",
                columns: table => new
                {
                    id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    store_id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    student_id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    voucher_item_id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    type = table.Column<int>(type: "int", nullable: true),
                    date_created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    date_updated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    state = table.Column<bool>(type: "bit", nullable: true),
                    status = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_activity", x => x.id);
                    table.ForeignKey(
                        name: "FK_tbl_activity_tbl_store_store_id",
                        column: x => x.store_id,
                        principalTable: "store",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbl_activity_tbl_student_student_id",
                        column: x => x.student_id,
                        principalTable: "student",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_tbl_activity_tbl_voucher_item_voucher_item_id",
                        column: x => x.voucher_item_id,
                        principalTable: "voucher_item",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "activity_transaction",
                columns: table => new
                {
                    id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    activity_id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    wallet_id = table.Column<string>(type: "char(26)", unicode: false, fixedLength: true, maxLength: 26, nullable: false),
                    amount = table.Column<decimal>(type: "decimal(38,2)", nullable: true),
                    rate = table.Column<decimal>(type: "decimal(38,2)", nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    state = table.Column<bool>(type: "bit", nullable: true),
                    status = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_activity_transaction", x => x.id);
                    table.ForeignKey(
                        name: "FK_tbl_activity_transaction_tbl_activity_activity_id",
                        column: x => x.activity_id,
                        principalTable: "activity",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbl_activity_transaction_tbl_wallet_wallet_id",
                        column: x => x.wallet_id,
                        principalTable: "wallet",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_tbl_activity_store_id",
                table: "activity",
                column: "store_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_activity_student_id",
                table: "activity",
                column: "student_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_activity_voucher_item_id",
                table: "activity",
                column: "voucher_item_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_activity_transaction_activity_id",
                table: "activity_transaction",
                column: "activity_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_activity_transaction_wallet_id",
                table: "activity_transaction",
                column: "wallet_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_admin_account_id",
                table: "admin",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_brand_account_id",
                table: "brand",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_campaign_brand_id",
                table: "campaign",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_campaign_type_id",
                table: "campaign",
                column: "type_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_campaign_campus_campaign_id",
                table: "campaign_campus",
                column: "campaign_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_campaign_campus_campus_id",
                table: "campaign_campus",
                column: "campus_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_campaign_detail_campaign_id",
                table: "campaign_detail",
                column: "campaign_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_campaign_detail_voucher_id",
                table: "campaign_detail",
                column: "voucher_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_campaign_store_campaign_id",
                table: "campaign_store",
                column: "campaign_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_campaign_store_store_id",
                table: "campaign_store",
                column: "store_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_campaign_transaction_campaign_id",
                table: "campaign_transaction",
                column: "campaign_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_campaign_transaction_wallet_id",
                table: "campaign_transaction",
                column: "wallet_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_campus_area_id",
                table: "campus",
                column: "area_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_challenge_transaction_challenge_id",
                table: "challenge_transaction",
                column: "challenge_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_challenge_transaction_wallet_id",
                table: "challenge_transaction",
                column: "wallet_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_invitation_invitee_id",
                table: "invitation",
                column: "invitee_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_invitation_inviter_id",
                table: "invitation",
                column: "inviter_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_staff_account_id",
                table: "lecturer",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_order_station_id",
                table: "order",
                column: "station_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_order_student_id",
                table: "order",
                column: "student_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_order_detail_order_id",
                table: "order_detail",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_order_detail_product_id",
                table: "order_detail",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_order_state_order_id",
                table: "order_state",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_order_transaction_order_id",
                table: "order_transaction",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_order_transaction_wallet_id",
                table: "order_transaction",
                column: "wallet_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_product_category_id",
                table: "product",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_request_admin_id",
                table: "request",
                column: "admin_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_request_brand_id",
                table: "request",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_request_transaction_request_id",
                table: "request_transaction",
                column: "request_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_request_transaction_wallet_id",
                table: "request_transaction",
                column: "wallet_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_bonus_brand_id",
                table: "reward",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_bonus_store_id",
                table: "reward",
                column: "store_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_bonus_student_id",
                table: "reward",
                column: "student_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_bonus_transaction_bonus_id",
                table: "reward_transaction",
                column: "bonus_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_bonus_transaction_wallet_id",
                table: "reward_transaction",
                column: "wallet_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_store_account_id",
                table: "store",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_store_area_id",
                table: "store",
                column: "area_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_store_brand_id",
                table: "store",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_student_account_id",
                table: "student",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_student_campus_id",
                table: "student",
                column: "campus_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_student_challenge_challenge_id",
                table: "student_challenge",
                column: "challenge_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_student_challenge_student_id",
                table: "student_challenge",
                column: "student_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_voucher_brand_id",
                table: "voucher",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_voucher_type_id",
                table: "voucher",
                column: "type_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_voucher_item_campaign_detail_id",
                table: "voucher_item",
                column: "campaign_detail_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_voucher_item_voucher_id",
                table: "voucher_item",
                column: "voucher_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_wallet_brand_id",
                table: "wallet",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_wallet_campaign_id",
                table: "wallet",
                column: "campaign_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_wallet_student_id",
                table: "wallet",
                column: "student_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "activity_transaction");

            migrationBuilder.DropTable(
                name: "campaign_campus");

            migrationBuilder.DropTable(
                name: "campaign_store");

            migrationBuilder.DropTable(
                name: "campaign_transaction");

            migrationBuilder.DropTable(
                name: "challenge_transaction");

            migrationBuilder.DropTable(
                name: "invitation");

            migrationBuilder.DropTable(
                name: "lecturer");

            migrationBuilder.DropTable(
                name: "order_detail");

            migrationBuilder.DropTable(
                name: "order_state");

            migrationBuilder.DropTable(
                name: "order_transaction");

            migrationBuilder.DropTable(
                name: "request_transaction");

            migrationBuilder.DropTable(
                name: "reward_transaction");

            migrationBuilder.DropTable(
                name: "activity");

            migrationBuilder.DropTable(
                name: "student_challenge");

            migrationBuilder.DropTable(
                name: "product");

            migrationBuilder.DropTable(
                name: "order");

            migrationBuilder.DropTable(
                name: "request");

            migrationBuilder.DropTable(
                name: "reward");

            migrationBuilder.DropTable(
                name: "wallet");

            migrationBuilder.DropTable(
                name: "voucher_item");

            migrationBuilder.DropTable(
                name: "challenge");

            migrationBuilder.DropTable(
                name: "category");

            migrationBuilder.DropTable(
                name: "station");

            migrationBuilder.DropTable(
                name: "admin");

            migrationBuilder.DropTable(
                name: "store");

            migrationBuilder.DropTable(
                name: "student");

            migrationBuilder.DropTable(
                name: "campaign_detail");

            migrationBuilder.DropTable(
                name: "campus");

            migrationBuilder.DropTable(
                name: "campaign");

            migrationBuilder.DropTable(
                name: "voucher");

            migrationBuilder.DropTable(
                name: "area");

            migrationBuilder.DropTable(
                name: "campaign_type");

            migrationBuilder.DropTable(
                name: "brand");

            migrationBuilder.DropTable(
                name: "voucher_type");

            migrationBuilder.DropTable(
                name: "account");
        }
    }
}
