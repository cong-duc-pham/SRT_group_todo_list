using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SRT_group_todo_list.Models;

namespace SRT_group_todo_list.Services
{
    public interface ITodoService
    {
        Task<IEnumerable<TodoItem>> GetAllAsync(string? search, bool? isCompleted, string? sortBy);
        Task<TodoItem?> GetByIdAsync(Guid id);
        Task<TodoItem> AddAsync(TodoItem item);
        Task<TodoItem?> UpdateAsync(TodoItem item);
        Task<bool> DeleteAsync(Guid id);
        Task<TodoItem?> ToggleStatusAsync(Guid id);
    }
}
