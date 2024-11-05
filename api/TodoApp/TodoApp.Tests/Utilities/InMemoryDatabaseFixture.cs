using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TodoApp.Api.Data;
using TodoApp.Api.Models;
using TodoApp.Api.Repositories;

namespace TodoApp.Tests.Utilities
{
    public class InMemoryDatabaseFixture : IDisposable
    {
        public AppDbContext Context { get; }
        public ITodoRepository Repository { get; }

        private readonly SqliteConnection _connection;

        public InMemoryDatabaseFixture()
        {
            // Initialize SQLite in-memory connection
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            // Configure DbContext options
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connection)
                .Options;

            // Initialize DbContext
            Context = new AppDbContext(options);
            Context.Database.EnsureCreated();

            // Initialize Repository
            Repository = new TodoRepository(Context);

            // Seed initial data if necessary
            SeedData();
        }

        private void SeedData()
        {
            var todos = new List<TodoItem>
            {
                new TodoItem { Id = Guid.NewGuid(), Title = "Test Todo 1", Description = "Description 1", IsCompleted = false },
                new TodoItem { Id = Guid.NewGuid(), Title = "Test Todo 2", Description = "Description 2", IsCompleted = true }
            };

            Context.TodoItems.AddRange(todos);
            Context.SaveChanges();
        }

        public void Dispose()
        {
            Context.Dispose();
            _connection.Close();
            _connection.Dispose();
        }
    }
}
