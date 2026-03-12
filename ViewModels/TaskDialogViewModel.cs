using System;
using System.Windows.Input;
using TaskManager.Models;

namespace TaskManager.ViewModels;

public class TaskDialogViewModel : BaseViewModel
{
    private string _title = string.Empty;
    private string? _description;
    private TaskPriority _priority = TaskPriority.Medium;
    private DateTime? _dueDate;
    private string? _validationError;

    public string Title
    {
        get => _title;
        set
        {
            SetProperty(ref _title, value);
            Validate();
        }
    }

    public string? Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    public TaskPriority Priority
    {
        get => _priority;
        set => SetProperty(ref _priority, value);
    }

    public DateTime? DueDate
    {
        get => _dueDate;
        set
        {
            SetProperty(ref _dueDate, value);
            Validate();
        }
    }

    public string? ValidationError
    {
        get => _validationError;
        private set => SetProperty(ref _validationError, value);
    }

    public bool IsValid =>
        !string.IsNullOrWhiteSpace(Title) &&
        (!DueDate.HasValue || DueDate.Value.Date >= DateTime.Today || IsEditMode);

    public bool IsEditMode { get; }
    public string DialogTitle => IsEditMode ? "Редактировать задачу" : "Новая задача";
    public TaskPriority[] Priorities { get; } = (TaskPriority[])Enum.GetValues(typeof(TaskPriority));

    public bool? DialogResult { get; private set; }
    public event EventHandler? CloseRequested;

    public ICommand ConfirmCommand { get; }
    public ICommand CancelCommand { get; }

    public TaskDialogViewModel(TaskItem? existing = null)
    {
        IsEditMode = existing is not null;
        if (existing is not null)
        {
            _title = existing.Title;
            _description = existing.Description;
            _priority = existing.Priority;
            _dueDate = existing.DueDate;
        }

        ConfirmCommand = new RelayCommand(Confirm, () => IsValid);
        CancelCommand = new RelayCommand(Cancel);
    }

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(Title))
            ValidationError = "Название обязательно.";
        else if (DueDate.HasValue && DueDate.Value.Date < DateTime.Today && !IsEditMode)
            ValidationError = "Срок не может быть в прошлом.";
        else
            ValidationError = null;

        OnPropertyChanged(nameof(IsValid));
    }

    private void Confirm() { DialogResult = true; CloseRequested?.Invoke(this, EventArgs.Empty); }
    private void Cancel() { DialogResult = false; CloseRequested?.Invoke(this, EventArgs.Empty); }

    public TaskItem ApplyTo(TaskItem? target = null)
    {
        var item = target ?? new TaskItem { CreatedAt = DateTime.Now };
        item.Title = Title.Trim();
        item.Description = string.IsNullOrWhiteSpace(Description) ? null : Description.Trim();
        item.Priority = Priority;
        item.DueDate = DueDate;
        return item;
    }
}
