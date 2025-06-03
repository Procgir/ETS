using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;
using Bogus;
using Dapper;
using ElectronicTestSystem.Application.Abstractions.Data;
using Npgsql;
using NpgsqlTypes;

namespace ElectronicTestSystem.WebApi.Extensions;

internal static class SeedDataExtensions
{
    public static void SeedData(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();
    
        ISqlConnectionFactory sqlConnectionFactory = scope.ServiceProvider.GetRequiredService<ISqlConnectionFactory>();
        using IDbConnection connection = sqlConnectionFactory.CreateConnection();
       
        var faker = new Faker();
        
        var fakeTeacherId = AddTeacher(faker, connection);

        var studentIds = AddStudents(faker, connection);
        
        var groupIds = AddGroups(faker, connection);

        AddGroupUsers(faker, connection, studentIds, groupIds);

        AddTests(fakeTeacherId, faker, connection);
    }

    private static void AddGroupUsers(Faker faker, IDbConnection connection, List<Guid> studentIds, List<Guid> groupIds)
    {
        List<object> userGroups = new();
        for (int i = 0; i < studentIds.Count; i++)
        {
            var groupId = groupIds[i % groupIds.Count];
            userGroups.Add(new
            {
                GroupsId = groupId,
                UsersId = studentIds[i]
            });
        }
        
        const string sql = """
           INSERT INTO public.group_user
           (groups_id, users_id)
           VALUES(@GroupsId, @UsersId);
           """;
        
        connection.Execute(sql, userGroups);
    }

    private static List<Guid> AddGroups(Faker faker, IDbConnection connection)
    {
        List<Guid> ids = new();
        List<object> groups = new();
        for (int i = 0; i < 5; i++)
        {
            var id = Guid.NewGuid();
            ids.Add(id);
            groups.Add(new
            {
                Id = id,
                NameValue = faker.Company.CompanyName()
            });
        }
        
        const string sql = """
           INSERT INTO public.groups
           (id, name_value)
           VALUES(@Id, @NameValue);
           """;
        
        connection.Execute(sql, groups);
        return ids;
    }

    private static void AddTests(Guid fakeTeacherId, Faker faker, IDbConnection connection)
    {
        var testIds = new List<Guid>();
        List<object> tests = new();
        for (int i = 0; i < 10; i++)
        {
            var id = Guid.NewGuid();
            testIds.Add(id);
            tests.Add(new
            {
                Id = id,
                CreatedAt = DateTime.Now,
                AuthorId = fakeTeacherId,
                SubjectName = faker.Hacker.Noun(),
                ThemeName = faker.Hacker.Noun()
            });
        }
        
        const string sql = """
           INSERT INTO public.tests
           (id, created_at, author_id, subject_name, theme_name)
           VALUES(@Id, @CreatedAt, @AuthorId, @SubjectName, @ThemeName);
           """;
        
        connection.Execute(sql, tests);
        
        List<object> testQuestions = new();
        for (int i = 0; i < testIds.Count; i++)
        {
            AddTestQuestions(faker, connection, testIds[i]);
        }
    }

    private static void AddTestQuestions(Faker faker, IDbConnection connection, Guid testId)
    {
        List<object> testQuestions = new();
        for (int j = 0; j < 20; j++)
        {
            var answers = Enumerable.Range(0, 4).Select(i => faker.Lorem.Word()).ToArray(); 
            testQuestions.Add(new
            {
                TestId = testId,
                Id = Guid.NewGuid(),
                Answers = new JsonBParameter(JsonSerializer.Serialize(answers)),
                BodyText = faker.Lorem.Sentence(10),
                TrueAnswerNumberValue = faker.Database.Random.Int(1, 4)
            });
        }
        const string sql = """
               INSERT INTO public.test_questions
               (id, test_id, answers, body_text, true_answer_number_value)
               VALUES(@Id, @TestId, @Answers, @BodyText, @TrueAnswerNumberValue);
               """;
        
        connection.Execute(sql, testQuestions);
    }

    private static List<Guid> AddStudents(Faker faker, IDbConnection connection)
    {
        List<Guid> ids = new();
        List<object> fakeStudents = new();
        for (int i = 0; i < 41; i++)
        {
            var fakerName = faker.Name;
            var id = Guid.NewGuid();
            ids.Add(id);
            fakeStudents.Add(new
            {
                Id = id,
                IsTeacher = false,
                LoginValue = $"test_{fakerName.FullName()}",
                NameFirstName = fakerName.FirstName(),
                NameMiddleName = string.Empty,
                NameLastName = fakerName.LastName(),
                PasswordHash = faker.Random.Hash(),
                CreatedAt = DateTime.Now
            });
        }
        
        const string sql = """
           INSERT INTO public.users
           (id, is_teacher, login_value, name_first_name, name_middle_name, name_last_name, password_hash)
           VALUES(@Id, @IsTeacher, @LoginValue, @NameFirstName, @NameMiddleName, @NameLastName, @PasswordHash);
           """;
    
        connection.Execute(sql, fakeStudents);
        
        return ids;
    }

    private static Guid AddTeacher(Faker faker, IDbConnection connection)
    {
        var id = Guid.NewGuid();
        var fakeTeacher = new
        {
            Id = id,
            IsTeacher = true,
            LoginValue = "test_teacher",
            NameFirstName = faker.Name.FirstName(),
            NameMiddleName = string.Empty,
            NameLastName = faker.Name.LastName(),
            PasswordHash = faker.Random.Hash()
        };
        
        const string sql = """
           INSERT INTO public.users
           (id, is_teacher, login_value, name_first_name, name_middle_name, name_last_name, password_hash)
           VALUES(@Id, @IsTeacher, @LoginValue, @NameFirstName, @NameMiddleName, @NameLastName, @PasswordHash);
           """;
    
        connection.Execute(sql, fakeTeacher);
        return id;
    }
    
    public class JsonBParameter : SqlMapper.ICustomQueryParameter
    {
        private readonly string json;

        public JsonBParameter(string json)
        {
            this.json = json;
        }

        public void AddParameter(IDbCommand command, string name)
        {
            var parameter = (NpgsqlParameter)command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = json;
            parameter.NpgsqlDbType = NpgsqlDbType.Jsonb;
            command.Parameters.Add(parameter);
        }
    }
}
