using SQLite;
using ToDoApp.Models;

namespace ToDoApp.Services;

public class TodoDatabase
{
    private readonly SQLiteAsyncConnection _db;

    public TodoDatabase(string dbPath)
    {
        _db = new SQLiteAsyncConnection(dbPath);
        _db.CreateTableAsync<TodoItem>().Wait();
    }

    public Task<List<TodoItem>> GetItemsAsync() =>
        _db.Table<TodoItem>().OrderByDescending(t => t.Id).ToListAsync();

    public Task<int> SaveItemAsync(TodoItem item) =>
        item.Id != 0 ? _db.UpdateAsync(item) : _db.InsertAsync(item);

    public Task<int> DeleteItemAsync(TodoItem item) =>
        _db.DeleteAsync(item);
}
