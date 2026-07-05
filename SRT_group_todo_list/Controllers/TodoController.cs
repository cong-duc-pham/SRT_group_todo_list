using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SRT_group_todo_list.Models;
using SRT_group_todo_list.Services;

namespace SRT_group_todo_list.Controllers
{
    public class TodoController : Controller
    {
        private readonly ITodoService _todoService;
        private readonly ILogger<TodoController> _logger;

        public TodoController(ITodoService todoService, ILogger<TodoController> logger)
        {
            _todoService = todoService;
            _logger = logger;
        }

        // GET: /Todo
        // GET: /Todo/Index
        public async Task<IActionResult> Index(string? search, bool? isCompleted, string? sortBy)
        {
            try
            {
                var todos = await _todoService.GetAllAsync(search, isCompleted, sortBy);

                // Pass query parameters back to View to populate search, filters, and sorting forms
                ViewData["CurrentSearch"] = search;
                ViewData["CurrentFilter"] = isCompleted;
                ViewData["CurrentSort"] = sortBy ?? "newest";

                return View(todos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching todo items.");
                return View("Error", new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
            }
        }

        // GET: /Todo/GetTodo/{id}
        [HttpGet]
        public async Task<IActionResult> GetTodo(Guid id)
        {
            var todo = await _todoService.GetByIdAsync(id);
            if (todo == null)
            {
                return NotFound(new { success = false, message = "Todo item not found." });
            }
            return Json(todo);
        }

        // POST: /Todo/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,DueDate")] TodoItem todo)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return Json(new { success = false, message = "Validation failed.", errors });
            }

            try
            {
                var result = await _todoService.AddAsync(todo);
                return Json(new { success = true, message = "Task created successfully.", data = result });
            }
            catch (ArgumentException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a todo item.");
                return Json(new { success = false, message = "An internal error occurred." });
            }
        }

        // POST: /Todo/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Title,Description,IsCompleted,DueDate")] TodoItem todo)
        {
            if (id != todo.Id)
            {
                return Json(new { success = false, message = "ID mismatch." });
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return Json(new { success = false, message = "Validation failed.", errors });
            }

            try
            {
                var result = await _todoService.UpdateAsync(todo);
                if (result == null)
                {
                    return Json(new { success = false, message = "Task not found." });
                }
                return Json(new { success = true, message = "Task updated successfully.", data = result });
            }
            catch (ArgumentException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating the todo item.");
                return Json(new { success = false, message = "An internal error occurred." });
            }
        }

        // POST: /Todo/ToggleStatus/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            try
            {
                var result = await _todoService.ToggleStatusAsync(id);
                if (result == null)
                {
                    return Json(new { success = false, message = "Task not found." });
                }
                return Json(new { success = true, message = "Status toggled successfully.", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while toggling the status of todo item.");
                return Json(new { success = false, message = "An internal error occurred." });
            }
        }

        // POST: /Todo/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var success = await _todoService.DeleteAsync(id);
                if (!success)
                {
                    return Json(new { success = false, message = "Task not found." });
                }
                return Json(new { success = true, message = "Task deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting the todo item.");
                return Json(new { success = false, message = "An internal error occurred." });
            }
        }
    }
}
