using Altametrics_Backend_C__.NET.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;

namespace Altametrics_Backend_C__.NET.Database
{
    public class DBInitializer
    {

        public static void RunInitSchema(IHost app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDBContext>();
            var conn = context.Database.GetDbConnection();

            conn.Open();
            using var checkCmd = conn.CreateCommand();
            checkCmd.CommandText = "SELECT COUNT(*) FROM information_schema.tables WHERE table_name = 'dbinitialized';";

            var exists = (long)(checkCmd.ExecuteScalar() ?? 0);

            if (exists == 0)
            {
                Console.WriteLine("Running schema initialization");

                var sql = File.ReadAllText("Database/initial_schema.sql");

                using var initCmd = conn.CreateCommand();
                initCmd.CommandText = sql;
                initCmd.ExecuteNonQuery();

                Console.WriteLine("Database initialized.");
            }
            else
            {
                Console.WriteLine("Schema already initialized. Skipping.");
            }

            conn.Close();
        }


        public static void EnsureDatabaseExists(string connectionString)
        {
            var builder = new NpgsqlConnectionStringBuilder(connectionString);
            var databaseName = builder.Database;

            builder.Database = "postgres";
            var masterConnStr = builder.ToString();

            using var conn = new NpgsqlConnection(masterConnStr);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = $"SELECT 1 FROM pg_database WHERE datname = @name";
            cmd.Parameters.AddWithValue("name", databaseName);
            var exists = cmd.ExecuteScalar();

            if (exists == null)
            {
                using var createCmd = conn.CreateCommand();
                createCmd.CommandText = $"CREATE DATABASE \"{databaseName}\"";
                createCmd.ExecuteNonQuery();

                Console.WriteLine($"Database '{databaseName}' created.");
            }
            else
            {
                Console.WriteLine($"Database '{databaseName}' already exists.");
            }

            conn.Close();
        }
    }


}
