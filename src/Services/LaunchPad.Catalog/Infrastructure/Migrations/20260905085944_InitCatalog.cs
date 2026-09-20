using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LaunchPad.Catalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "agencies",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ll2id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    abbrev = table.Column<string>(type: "text", nullable: true),
                    type = table.Column<short>(type: "smallint", nullable: false),
                    country_code = table.Column<string>(type: "character(2)", fixedLength: true, maxLength: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_agencies", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "pads",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ll2id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    country_code = table.Column<string>(type: "character(2)", fixedLength: true, maxLength: 2, nullable: true),
                    latitude = table.Column<double>(type: "double precision", nullable: true),
                    longitude = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pads", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "rocket_configs",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ll2id = table.Column<Guid>(type: "uuid", nullable: false),
                    full_name = table.Column<string>(type: "text", nullable: false),
                    family = table.Column<string>(type: "text", nullable: true),
                    reusable_stages = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_rocket_configs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "launches",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ll2id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    net_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    window_start_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    window_end_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    agency_id = table.Column<long>(type: "bigint", nullable: false),
                    rocket_config_id = table.Column<long>(type: "bigint", nullable: false),
                    pad_id = table.Column<long>(type: "bigint", nullable: false),
                    mission_name = table.Column<string>(type: "text", nullable: true),
                    mission_orbit = table.Column<short>(type: "smallint", nullable: false),
                    last_synced_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_launches", x => x.id);
                    table.ForeignKey(
                        name: "fk_launches_agencies_agency_id",
                        column: x => x.agency_id,
                        principalTable: "agencies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_launches_pads_pad_id",
                        column: x => x.pad_id,
                        principalTable: "pads",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_launches_rocket_configs_rocket_config_id",
                        column: x => x.rocket_config_id,
                        principalTable: "rocket_configs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "launch_status_history",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    launch_id = table.Column<long>(type: "bigint", nullable: false),
                    change_type = table.Column<short>(type: "smallint", nullable: false),
                    old_net_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    new_net_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    old_status = table.Column<short>(type: "smallint", nullable: true),
                    new_status = table.Column<short>(type: "smallint", nullable: true),
                    detected_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_launch_status_history", x => x.id);
                    table.ForeignKey(
                        name: "fk_launch_status_history_launches_launch_id",
                        column: x => x.launch_id,
                        principalTable: "launches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_agencies_country_code",
                table: "agencies",
                column: "country_code");

            migrationBuilder.CreateIndex(
                name: "ix_agencies_ll2id",
                table: "agencies",
                column: "ll2id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_launch_status_history_launch_id",
                table: "launch_status_history",
                column: "launch_id");

            migrationBuilder.CreateIndex(
                name: "ix_launches_agency_id",
                table: "launches",
                column: "agency_id");

            migrationBuilder.CreateIndex(
                name: "ix_launches_ll2id",
                table: "launches",
                column: "ll2id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_launches_net_utc",
                table: "launches",
                column: "net_utc");

            migrationBuilder.CreateIndex(
                name: "ix_launches_pad_id",
                table: "launches",
                column: "pad_id");

            migrationBuilder.CreateIndex(
                name: "ix_launches_rocket_config_id",
                table: "launches",
                column: "rocket_config_id");

            migrationBuilder.CreateIndex(
                name: "ix_launches_status_net_utc",
                table: "launches",
                columns: new[] { "status", "net_utc" });

            migrationBuilder.CreateIndex(
                name: "ix_pads_ll2id",
                table: "pads",
                column: "ll2id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_rocket_configs_family",
                table: "rocket_configs",
                column: "family");

            migrationBuilder.CreateIndex(
                name: "ix_rocket_configs_ll2id",
                table: "rocket_configs",
                column: "ll2id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "launch_status_history");

            migrationBuilder.DropTable(
                name: "launches");

            migrationBuilder.DropTable(
                name: "agencies");

            migrationBuilder.DropTable(
                name: "pads");

            migrationBuilder.DropTable(
                name: "rocket_configs");
        }
    }
}
