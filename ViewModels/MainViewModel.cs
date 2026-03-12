using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using TaskManager.Models;
using TaskManager.Services;
using TaskManager.Views;

namespace TaskManager.ViewModels;

public class MainViewModel : BaseViewModel
{
    private readonly ITaskService _service;
    private readonly ObservableCollection<TaskItem> _tasks = [];
    private readonly ICollectionView _view;

    private string _search = string.Empty;
    private string _filter = "Все";
    private string _sort = "Дата создания";
    private TaskItem? _selected;
    private bool _isLoading;
    private string? _error;

    public ICollectionView TasksView => _view;

    public string Search
    {
        get => _search;
        set { SetProperty(ref _search, value); _view.Refresh(); }
    }

    public string Filter
    {
        get => _filter;
        set { SetProperty(ref _filter, value); _view.Refresh(); }
    }

    public string Sort
    {
        get => _sort;
        set { SetProperty(ref _sort, value); ApplySorting(); }
    }

    public TaskItem? Selected
    {
        get => _selected;
        set => SetProperty(ref _selected, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public string? Error
    {
        get => _error;
        set => SetProperty(ref _error, value);
    }

    public string[] FilterOptions { get; } = ["Все", "Активные", "Завершённые"];
    public string[] SortOptions { get; } = ["Дата создания", "Приоритет", "Срок"];

    public ICommand AddCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand ToggleCommand { get; }

    public MainViewModel(ITaskService service)
    {
        _service = service;
        _view = CollectionViewSource.GetDefaultView(_tasks);
        _view.Filter = FilterPredicate;
        ApplySorting();

        AddCommand = new RelayCommand(_ => AddTask());
        EditCommand = new RelayCommand(p => EditTask(ResolveItem(p)), p => ResolveItem(p) is not null);
        DeleteCommand = new RelayCommand(p => DeleteTask(ResolveItem(p)), p => ResolveItem(p) is not null);
        ToggleCommand = new RelayCommand(p => ToggleStatus(p as TaskItem), p => p is TaskItem);

        _ = LoadAsync();
    }

    // Resolve item from parameter (row button) or from SelectedItem (toolbar button)
    private TaskItem? ResolveItem(object? param) => param as TaskItem ?? Selected;

    private bool FilterPredicate(object obj)
    {
        if (obj is not TaskItem t) return false;

        var matchSearch = string.IsNullOrWhiteSpace(Search) ||
            t.Title.Contains(Search, StringComparison.OrdinalIgnoreCase) ||
            (t.Description?.Contains(Search, StringComparison.OrdinalIgnoreCase) ?? false);

        var matchFilter = Filter switch
        {
            "Активные" => t.Status == TaskItemStatus.Active,
            "Завершённые" => t.Status == TaskItemStatus.Completed,
            _ => true
        };

        return matchSearch && matchFilter;
    }

    private void ApplySorting()
    {
        _view.SortDescriptions.Clear();
        var (prop, dir) = Sort switch
        {
            "Приоритет" => ("Priority", ListSortDirection.Descending),
            "Срок" => ("DueDate", ListSortDirection.Ascending),
            _ => ("CreatedAt", ListSortDirection.Descending)
        };
        _view.SortDescriptions.Add(new SortDescription(prop, dir));
    }

    private async Task LoadAsync()
    {
        IsLoading = true; Error = null;
        try
        {
            var items = await _service.LoadTasksAsync();
            _tasks.Clear();
            foreach (var i in items) _tasks.Add(i);
        }
        catch (Exception ex) { Error = $"Ошибка загрузки: {ex.Message}"; }
        finally { IsLoading = false; }
    }

    private async void SaveAsync()
    {
        try { await _service.SaveTasksAsync(_tasks); }
        catch (Exception ex) { Error = $"Ошибка сохранения: {ex.Message}"; }
    }

    private void AddTask()
    {
        var vm = new TaskDialogViewModel();
        OpenDialog(vm);
        if (vm.DialogResult == true)
        {
            _tasks.Add(vm.ApplyTo());
            SaveAsync();
        }
    }

    private void EditTask(TaskItem? item)
    {
        if (item is null) return;
        var vm = new TaskDialogViewModel(item);
        OpenDialog(vm);
        if (vm.DialogResult == true)
        {
            vm.ApplyTo(item);
            _view.Refresh();
            SaveAsync();
        }
    }

    private void DeleteTask(TaskItem? item)
    {
        if (item is null) return;
        var result = MessageBox.Show(
            $"Удалить задачу «{item.Title}»?",
            "Подтверждение удаления",
            MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (result == MessageBoxResult.Yes)
        {
            _tasks.Remove(item);
            SaveAsync();
        }
    }

    private void ToggleStatus(TaskItem? item)
    {
        if (item is null) return;
        item.Status = item.Status == TaskItemStatus.Active
            ? TaskItemStatus.Completed
            : TaskItemStatus.Active;
        _view.Refresh();
        SaveAsync();
    }

    private static void OpenDialog(TaskDialogViewModel vm)
    {
        var dlg = new TaskDialog { DataContext = vm };
        vm.CloseRequested += (_, _) => dlg.Close();
        dlg.Owner = Application.Current.MainWindow;
        dlg.ShowDialog();
    }
}
