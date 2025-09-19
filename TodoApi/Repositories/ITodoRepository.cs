using TodoApi.Models;

namespace TodoApi.Repositories
{
    public interface ITodoRepository
    {
        Task<IEnumerable<TodoItem>> GetAllAsync();
        Task<TodoItem?> GetAsync(long id);
        Task<TodoItem> CreateAsync(TodoItem item);
        Task<TodoItem?> UpdateAsync(long id, TodoItem item);
        Task<bool> DeleteAsync(long id);
    }
}