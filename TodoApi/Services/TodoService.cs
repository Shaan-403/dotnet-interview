using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using TodoApi.Interfaces;
using TodoApi.Models;

namespace TodoApi.Services
{
    public class TodoService : ITodoService
    {
        private readonly string _connectionString;

        public TodoService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("TodoDb")
                ?? throw new InvalidOperationException("Connection string 'TodoDb' not found.");
        }

        public Todo CreateTodo(Todo todo)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var createdAt = DateTime.UtcNow;

            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Todos (Title, Description, IsCompleted, CreatedAt)
                VALUES (@title, @description, @isCompleted, @createdAt);

                SELECT last_insert_rowid();
            ";

            command.Parameters.AddWithValue("@title", todo.Title);
            command.Parameters.AddWithValue("@description", todo.Description);
            command.Parameters.AddWithValue("@isCompleted", false);
            command.Parameters.AddWithValue("@createdAt", createdAt);

            todo.Id = Convert.ToInt32(command.ExecuteScalar());
            todo.CreatedAt = createdAt;
            todo.IsCompleted = false;

            return todo;
        }

        public List<Todo> GetAllTodos()
        {
            var todos = new List<Todo>();

            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Todos";

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                todos.Add(MapTodo(reader));
            }

            return todos;
        }

        public Todo? GetTodoById(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Todos WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);

            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                return MapTodo(reader);
            }

            return null;
        }

        public Todo? UpdateTodo(int id, Todo todo)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();

            command.CommandText = @"
                UPDATE Todos
                SET Title = @title,
                    Description = @description,
                    IsCompleted = @isCompleted
                WHERE Id = @id;
            ";

            command.Parameters.AddWithValue("@title", todo.Title);
            command.Parameters.AddWithValue("@description", todo.Description);
            command.Parameters.AddWithValue("@isCompleted", todo.IsCompleted ? 1 : 0);
            command.Parameters.AddWithValue("@id", id);

            var rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected == 0)
            {
                return null;
            }

            return GetTodoById(id);
        }

        public bool DeleteTodo(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Todos WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);

            return command.ExecuteNonQuery() > 0;
        }

        private static Todo MapTodo(SqliteDataReader reader)
        {
            return new Todo
            {
                Id = reader.GetInt32(0),
                Title = reader.GetString(1),
                Description = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                IsCompleted = reader.GetInt32(3) == 1,
                CreatedAt = DateTime.Parse(reader.GetString(4))
            };
        }
    }
}