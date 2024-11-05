using TodoApp.Api.Data;
using TodoApp.Api.Models;

namespace TodoApp.Api.Repositories
{
    public class TodoRepository : Repository<TodoItem>, ITodoRepository
    {
        public TodoRepository(AppDbContext context) : base(context)
        {
        }
    }
}
