using System.Xml.Linq;
using TodoApi.Models;

namespace TodoApi.Repositories
{
    public class InMemoryTodoRepository : ITodoRepository
    {
        private readonly List<TodoItem> _items = new();
        private long _nextId = 1;

        public async Task<IEnumerable<TodoItem>> GetAllAsync()
        {
            return await Task.FromResult(_items);
        }

        public async Task<TodoItem?> GetAsync(long id)
        {
            return await Task.FromResult(_items.FirstOrDefault(i => i.Id == id));
        }

        public async Task<TodoItem> CreateAsync(TodoItem item)
        {
            item.Id = Interlocked.Increment(ref _nextId);
            _items.Add(item);
            return await Task.FromResult(item);
        }

        public async Task<TodoItem?> UpdateAsync(long id, TodoItem item)
        {
            var existing = _items.FirstOrDefault(i => i.Id == id);
            if (existing == null) return null;
            existing.Name = item.Name ?? string.Empty;
            existing.Description = item.Description ?? string.Empty;
            existing.IsComplete = item.IsComplete;
            existing.Status = item.Status;
            return await Task.FromResult(existing);
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var item = _items.FirstOrDefault(i => i.Id == id);
            if (item == null) return false;
            _items.Remove(item);
            return await Task.FromResult(true);
        }
    }
}