using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SRT_group_todo_list.Models;
using SRT_group_todo_list.Services;
using Xunit;

namespace SRT_group_todo_list.Tests
{
    public class TodoServiceTests
    {
        private TodoDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<TodoDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB name for each test
                .Options;

            return new TodoDbContext(options);
        }

        [Fact]
        public async Task AddAsync_ValidTodo_ShouldAddTaskToDatabase()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var service = new TodoService(context);
            var todo = new TodoItem
            {
                Title = "Test Task",
                Description = "Test Description",
                DueDate = DateTime.Today.AddDays(1)
            };

            // Act
            var result = await service.AddAsync(todo);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.Id);
            Assert.Equal("Test Task", result.Title);
            Assert.False(result.IsCompleted);

            var dbItem = await context.TodoItems.FindAsync(result.Id);
            Assert.NotNull(dbItem);
            Assert.Equal("Test Task", dbItem.Title);
        }

        [Fact]
        public async Task AddAsync_EmptyTitle_ShouldThrowArgumentException()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var service = new TodoService(context);
            var todo = new TodoItem
            {
                Title = "   ", // Empty title
                Description = "Test"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.AddAsync(todo));
        }

        [Fact]
        public async Task AddAsync_DueDateInPast_ShouldThrowArgumentException()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var service = new TodoService(context);
            var todo = new TodoItem
            {
                Title = "Test Task",
                DueDate = DateTime.Today.AddDays(-1) // Past due date
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.AddAsync(todo));
        }

        [Fact]
        public async Task GetAllAsync_SearchFilter_ShouldReturnMatchingItems()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            await context.TodoItems.AddRangeAsync(
                new TodoItem { Title = "Buy groceries", Description = "Milk and bread" },
                new TodoItem { Title = "Call doctor", Description = "Checkup booking" },
                new TodoItem { Title = "Buy a book", Description = "Programming guide" }
            );
            await context.SaveChangesAsync();

            var service = new TodoService(context);

            // Act
            var results = await service.GetAllAsync("buy", null, null);

            // Assert
            var list = results.ToList();
            Assert.Equal(2, list.Count);
            Assert.Contains(list, t => t.Title == "Buy groceries");
            Assert.Contains(list, t => t.Title == "Buy a book");
            Assert.DoesNotContain(list, t => t.Title == "Call doctor");
        }

        [Fact]
        public async Task GetAllAsync_CompletedFilter_ShouldReturnOnlyCompletedItems()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            await context.TodoItems.AddRangeAsync(
                new TodoItem { Title = "Task 1", IsCompleted = true },
                new TodoItem { Title = "Task 2", IsCompleted = false },
                new TodoItem { Title = "Task 3", IsCompleted = true }
            );
            await context.SaveChangesAsync();

            var service = new TodoService(context);

            // Act
            var results = await service.GetAllAsync(null, true, null);

            // Assert
            var list = results.ToList();
            Assert.Equal(2, list.Count);
            Assert.All(list, t => Assert.True(t.IsCompleted));
        }

        [Fact]
        public async Task ToggleStatusAsync_ExistingTodo_ShouldToggleCompletionState()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var todo = new TodoItem { Title = "Test Task", IsCompleted = false };
            await context.TodoItems.AddAsync(todo);
            await context.SaveChangesAsync();

            var service = new TodoService(context);

            // Act & Assert (Toggle 1: false -> true)
            var result1 = await service.ToggleStatusAsync(todo.Id);
            Assert.NotNull(result1);
            Assert.True(result1.IsCompleted);

            // Act & Assert (Toggle 2: true -> false)
            var result2 = await service.ToggleStatusAsync(todo.Id);
            Assert.NotNull(result2);
            Assert.False(result2.IsCompleted);
        }

        [Fact]
        public async Task DeleteAsync_ExistingTodo_ShouldRemoveTask()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var todo = new TodoItem { Title = "Task to delete" };
            await context.TodoItems.AddAsync(todo);
            await context.SaveChangesAsync();

            var service = new TodoService(context);

            // Act
            var success = await service.DeleteAsync(todo.Id);

            // Assert
            Assert.True(success);
            var exists = await context.TodoItems.AnyAsync(t => t.Id == todo.Id);
            Assert.False(exists);
        }

        [Fact]
        public async Task UpdateAsync_ValidTodo_ShouldModifyFields()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var todo = new TodoItem { Title = "Original Title", Description = "Original Desc" };
            await context.TodoItems.AddAsync(todo);
            await context.SaveChangesAsync();

            var service = new TodoService(context);

            // Act
            var updatedTodo = new TodoItem
            {
                Id = todo.Id,
                Title = "Updated Title",
                Description = "Updated Desc",
                IsCompleted = true
            };
            var result = await service.UpdateAsync(updatedTodo);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Title", result.Title);
            Assert.Equal("Updated Desc", result.Description);
            Assert.True(result.IsCompleted);

            var dbItem = await context.TodoItems.FindAsync(todo.Id);
            Assert.NotNull(dbItem);
            Assert.Equal("Updated Title", dbItem.Title);
        }
    }
}
