$(document).ready(function () {
    // Helper function to get anti-forgery token
    function getVerificationToken() {
        return $('input[name="__RequestVerificationToken"]').val();
    }

    // --- CREATE TODO ITEM ---
    $('#createTodoForm').on('submit', function (e) {
        e.preventDefault();
        
        const form = $(this);
        if (!form.valid()) return; // Exit if jQuery Validation fails client-side

        $.ajax({
            url: form.attr('action'),
            type: 'POST',
            data: form.serialize(),
            success: function (response) {
                if (response.success) {
                    showToast('Success', response.message, 'success');
                    $('#createTodoModal').modal('hide');
                    form[0].reset();
                    // Reload page to reflect new item in SSR sorted list
                    setTimeout(() => window.location.reload(), 600);
                } else {
                    showToast('Error', response.message, 'danger');
                }
            },
            error: function () {
                showToast('Error', 'An unexpected error occurred.', 'danger');
            }
        });
    });

    // --- LOAD DATA INTO EDIT MODAL ---
    $(document).on('click', '.btn-edit-todo', function () {
        const id = $(this).data('id');
        
        $.ajax({
            url: `/Todo/GetTodo/${id}`,
            type: 'GET',
            success: function (todo) {
                $('#editId').val(todo.id);
                $('#editTitle').val(todo.title);
                $('#editDescription').val(todo.description);
                $('#editIsCompleted').prop('checked', todo.isCompleted);
                
                if (todo.dueDate) {
                    // Format date to yyyy-MM-dd for HTML5 date input
                    const formattedDate = todo.dueDate.split('T')[0];
                    $('#editDueDate').val(formattedDate);
                } else {
                    $('#editDueDate').val('');
                }
                
                $('#editTodoModal').modal('show');
            },
            error: function () {
                showToast('Error', 'Could not load task details.', 'danger');
            }
        });
    });

    // --- SAVE EDITED TODO ITEM ---
    $('#editTodoForm').on('submit', function (e) {
        e.preventDefault();
        
        const form = $(this);
        if (!form.valid()) return;

        const id = $('#editId').val();

        $.ajax({
            url: `/Todo/Edit/${id}`,
            type: 'POST',
            data: form.serialize(),
            success: function (response) {
                if (response.success) {
                    showToast('Success', response.message, 'success');
                    $('#editTodoModal').modal('hide');
                    setTimeout(() => window.location.reload(), 600);
                } else {
                    showToast('Error', response.message, 'danger');
                }
            },
            error: function () {
                showToast('Error', 'An unexpected error occurred.', 'danger');
            }
        });
    });

    // --- TOGGLE COMPLETION STATUS ---
    $(document).on('change', '.todo-checkbox', function () {
        const checkbox = $(this);
        const id = checkbox.data('id');
        const card = checkbox.closest('.todo-card');
        const token = getVerificationToken();

        $.ajax({
            url: `/Todo/ToggleStatus/${id}`,
            type: 'POST',
            data: {
                id: id,
                __RequestVerificationToken: token
            },
            success: function (response) {
                if (response.success) {
                    const isCompleted = response.data.isCompleted;
                    
                    if (isCompleted) {
                        card.addClass('todo-completed');
                    } else {
                        card.removeClass('todo-completed');
                    }
                    
                    showToast('Updated', 'Task status updated.', 'info');
                    updateTaskCounts();
                } else {
                    // Revert checkbox state on error
                    checkbox.prop('checked', !checkbox.prop('checked'));
                    showToast('Error', response.message, 'danger');
                }
            },
            error: function () {
                checkbox.prop('checked', !checkbox.prop('checked'));
                showToast('Error', 'Failed to update task status.', 'danger');
            }
        });
    });

    // --- DELETE TODO ITEM ---
    $(document).on('click', '.btn-delete-todo', function () {
        const id = $(this).data('id');
        const title = $(this).data('title');
        const token = getVerificationToken();

        if (confirm(`Are you sure you want to delete "${title}"?`)) {
            $.ajax({
                url: `/Todo/Delete/${id}`,
                type: 'POST',
                data: {
                    id: id,
                    __RequestVerificationToken: token
                },
                success: function (response) {
                    if (response.success) {
                        showToast('Deleted', response.message, 'success');
                        
                        // Select target card and animate removal
                        const card = $(`.todo-card[data-id="${id}"]`);
                        card.fadeOut(400, function () {
                            $(this).remove();
                            updateTaskCounts();
                            
                            // If list is empty, refresh to show empty state
                            if ($('.todo-card[data-id]').length === 0) {
                                window.location.reload();
                            }
                        });
                    } else {
                        showToast('Error', response.message, 'danger');
                    }
                },
                error: function () {
                    showToast('Error', 'Failed to delete task.', 'danger');
                }
            });
        }
    });

    // --- HELPER: UPDATE TABS & COUNTERS INSTANTLY ---
    function updateTaskCounts() {
        const total = $('.todo-card[data-id]').length;
        const completed = $('.todo-card.todo-completed[data-id]').length;
        const pending = total - completed;
        
        $('#count-total').text(total);
        $('#count-completed').text(completed);
        $('#count-pending').text(pending);
    }

    // --- HELPER: TOAST MESSAGES SYSTEM ---
    function showToast(title, message, type = 'info') {
        const toastId = 'toast-' + Date.now();
        const toastHtml = `
            <div id="${toastId}" class="toast align-items-center text-white bg-${type} border-0 mb-2" role="alert" aria-live="assertive" aria-atomic="true">
                <div class="d-flex">
                    <div class="toast-body">
                        <strong>${title}:</strong> ${message}
                    </div>
                    <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
                </div>
            </div>
        `;
        
        // Append toast container if not exists
        if ($('#toastContainer').length === 0) {
            $('body').append('<div id="toastContainer" class="toast-container position-fixed bottom-0 end-0 p-3" style="z-index: 1055;"></div>');
        }
        
        $('#toastContainer').append(toastHtml);
        const toastElement = new bootstrap.Toast(document.getElementById(toastId), { delay: 3000 });
        toastElement.show();
        
        // Remove toast from DOM after hidden
        $(`#${toastId}`).on('hidden.bs.toast', function () {
            $(this).remove();
        });
    }
});
