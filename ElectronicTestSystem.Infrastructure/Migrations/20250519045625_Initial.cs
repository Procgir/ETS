using System;
using System.Collections.Generic;
using ElectronicTestSystem.Domain.Tests;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectronicTestSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "groups",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name_value = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_groups", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tests",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    author_id = table.Column<Guid>(type: "uuid", nullable: false),
                    subject_name = table.Column<string>(type: "text", nullable: false),
                    theme_name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tests", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_teacher = table.Column<bool>(type: "boolean", nullable: false),
                    login_value = table.Column<string>(type: "text", nullable: false),
                    name_first_name = table.Column<string>(type: "text", nullable: false),
                    name_last_name = table.Column<string>(type: "text", nullable: false),
                    name_middle_name = table.Column<string>(type: "text", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "test_questions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    test_id = table.Column<Guid>(type: "uuid", nullable: false),
                    answers = table.Column<List<TestQuestionAnswerOption>>(type: "jsonb", nullable: false),
                    body_text = table.Column<string>(type: "text", nullable: false),
                    true_answer_number_value = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_test_questions", x => x.id);
                    table.ForeignKey(
                        name: "fk_test_questions_tests_test_id",
                        column: x => x.test_id,
                        principalTable: "tests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "group_user",
                columns: table => new
                {
                    groups_id = table.Column<Guid>(type: "uuid", nullable: false),
                    users_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_group_user", x => new { x.groups_id, x.users_id });
                    table.ForeignKey(
                        name: "fk_group_user_groups_groups_id",
                        column: x => x.groups_id,
                        principalTable: "groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_group_user_user_users_id",
                        column: x => x.users_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "testings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ended_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    test_id = table.Column<Guid>(type: "uuid", nullable: false),
                    group_id = table.Column<Guid>(type: "uuid", nullable: false),
                    author_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_testings", x => x.id);
                    table.ForeignKey(
                        name: "fk_testings_groups_group_id",
                        column: x => x.group_id,
                        principalTable: "groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_testings_tests_test_id",
                        column: x => x.test_id,
                        principalTable: "tests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_testings_user_author_id",
                        column: x => x.author_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "testing_user_answers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    testing_id = table.Column<Guid>(type: "uuid", nullable: false),
                    answers = table.Column<string>(type: "jsonb", nullable: false),
                    mark_value = table.Column<int>(type: "integer", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_testing_user_answers", x => x.id);
                    table.ForeignKey(
                        name: "fk_testing_user_answers_testings_testing_id",
                        column: x => x.testing_id,
                        principalTable: "testings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_testing_user_answers_user_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_group_user_users_id",
                table: "group_user",
                column: "users_id");

            migrationBuilder.CreateIndex(
                name: "ix_test_questions_test_id",
                table: "test_questions",
                column: "test_id");

            migrationBuilder.CreateIndex(
                name: "ix_testing_user_answers_testing_id",
                table: "testing_user_answers",
                column: "testing_id");

            migrationBuilder.CreateIndex(
                name: "ix_testing_user_answers_user_id",
                table: "testing_user_answers",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_testings_author_id",
                table: "testings",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "ix_testings_group_id",
                table: "testings",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "ix_testings_test_id",
                table: "testings",
                column: "test_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "group_user");

            migrationBuilder.DropTable(
                name: "test_questions");

            migrationBuilder.DropTable(
                name: "testing_user_answers");

            migrationBuilder.DropTable(
                name: "testings");

            migrationBuilder.DropTable(
                name: "groups");

            migrationBuilder.DropTable(
                name: "tests");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
