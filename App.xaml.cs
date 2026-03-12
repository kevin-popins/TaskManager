using System.Windows;
using TaskManager.Services;
using TaskManager.ViewModels;
using TaskManager.Views;

namespace TaskManager;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        var service = new JsonTaskService();
        var vm = new MainViewModel(service);
        var window = new MainWindow { DataContext = vm };
        window.Show();
    }
}
