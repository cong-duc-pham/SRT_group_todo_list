using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SRT_group_todo_list.Models;

namespace SRT_group_todo_list.Services
{
    public class TodoService : ITodoService
    {
        private readonly TodoDbContext _context;

        public TodoService(TodoDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TodoItem>> GetAllAsync(string? search, bool? isCompleted, string? sortBy)
        {
            var query = _context.TodoItems.AsNoTracking().AsQueryable();

            // Filter by search keyword (case-insensitive search in Title or Description)
            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.Trim().ToLower();
                query = query.Where(t => t.Title.ToLower().Contains(searchLower) 
                                      || (t.Description != null && t.Description.ToLower().Contains(searchLower)));
            }

            // Filter by completion status
            if (isCompleted.HasValue)
            {
                query = query.Where(t => t.IsCompleted == isCompleted.Value);
            }

            // Sorting logic
            query = sortBy switch
            {
                "title" => query.OrderBy(t => t.Title),
                "title_desc" => query.OrderByDescending(t => t.Title),
                "due_date" => query.OrderBy(t => t.DueDate.HasValue).ThenBy(t => t.DueDate), // Nulls last, ascending due date
                "due_date_desc" => query.OrderByDescending(t => t.DueDate.HasValue).ThenByDescending(t => t.DueDate),
                "oldest" => query.OrderBy(t => t.CreatedAt),
                _ => query.OrderByDescending(t => t.CreatedAt) // Default sorting: newest first
            };

            return await query.ToListAsync();
        }

        public async Task<TodoItem?> GetByIdAsync(Guid id)
        {
            return await _context.TodoItems.FindAsync(id);
        }

        public async Task<TodoItem> AddAsync(TodoItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "Todo item cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(item.Title))
            {
                throw new ArgumentException("Todo title cannot be empty.", nameof(item.Title));
            }

            // Additional business validation: Due Date cannot be in the past when creating a new task
            if (item.DueDate.HasValue && item.DueDate.Value.Date < DateTime.Today)
            {
                throw new ArgumentException("Due Date cannot be in the past.", nameof(item.DueDate));
            }

            item.Id = Guid.NewGuid();
            item.CreatedAt = DateTime.Now;

            await _context.TodoItems.AddAsync(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<TodoItem?> UpdateAsync(TodoItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "Todo item cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(item.Title))
            {
                throw new ArgumentException("Todo title cannot be empty.", nameof(item.Title));
            }

            var existingItem = await _context.TodoItems.FindAsync(item.Id);
            if (existingItem == null)
            {
                return null;
            }

            // Update allowed properties
            existingItem.Title = item.Title;
            existingItem.Description = item.Description;
            existingItem.IsCompleted = item.IsCompleted;
            existingItem.DueDate = item.DueDate;

            _context.TodoItems.Update(existingItem);
            await _context.SaveChangesAsync();
            return existingItem;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var item = await _context.TodoItems.FindAsync(id);
            if (item == null)
            {
                return false;
            }

            _context.TodoItems.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<TodoItem?> ToggleStatusAsync(Guid id)
        {
            var item = await _context.TodoItems.FindAsync(id);
            if (item == null)
            {
                return null;
            }

            item.IsCompleted = !item.IsCompleted;
            await _context.SaveChangesAsync();
            return item;
        }
    }
}
