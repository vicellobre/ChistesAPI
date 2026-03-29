using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Enjoy.Persistence.Migrations.Application;

/// <inheritdoc />
public partial class InitialApplication : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "enjoy");

        migrationBuilder.CreateTable(
            name: "jokes",
            schema: "enjoy",
            columns: table => new
            {
                id = table.Column<string>(type: "text", nullable: false),
                text = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                author_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                origin = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                created_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                modified_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_jokes", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "outbox_message_consumers",
            schema: "enjoy",
            columns: table => new
            {
                id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_outbox_message_consumers", x => new { x.id, x.name });
            });

        migrationBuilder.CreateTable(
            name: "outbox_messages",
            schema: "enjoy",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                type = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                content = table.Column<string>(type: "text", nullable: false),
                occurred_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                processed_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                error = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_outbox_messages", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "topics",
            schema: "enjoy",
            columns: table => new
            {
                id = table.Column<string>(type: "text", nullable: false),
                name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_topics", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "users",
            schema: "enjoy",
            columns: table => new
            {
                id = table.Column<string>(type: "text", nullable: false),
                name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                email = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                password_hash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                role = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                created_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                modified_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                identity_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_users", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "joke_topics",
            schema: "enjoy",
            columns: table => new
            {
                joke_id = table.Column<string>(type: "text", nullable: false),
                topic_id = table.Column<string>(type: "text", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_joke_topics", x => new { x.joke_id, x.topic_id });
                table.ForeignKey(
                    name: "fk_joke_topics_jokes_joke_id",
                    column: x => x.joke_id,
                    principalSchema: "enjoy",
                    principalTable: "jokes",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_joke_topics_topics_topic_id",
                    column: x => x.topic_id,
                    principalSchema: "enjoy",
                    principalTable: "topics",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "ix_joke_topics_topic_id",
            schema: "enjoy",
            table: "joke_topics",
            column: "topic_id");

        migrationBuilder.CreateIndex(
            name: "ix_topics_name",
            schema: "enjoy",
            table: "topics",
            column: "name",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_users_email",
            schema: "enjoy",
            table: "users",
            column: "email",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_users_identity_id",
            schema: "enjoy",
            table: "users",
            column: "identity_id",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "joke_topics",
            schema: "enjoy");

        migrationBuilder.DropTable(
            name: "outbox_message_consumers",
            schema: "enjoy");

        migrationBuilder.DropTable(
            name: "outbox_messages",
            schema: "enjoy");

        migrationBuilder.DropTable(
            name: "users",
            schema: "enjoy");

        migrationBuilder.DropTable(
            name: "jokes",
            schema: "enjoy");

        migrationBuilder.DropTable(
            name: "topics",
            schema: "enjoy");
    }
}
