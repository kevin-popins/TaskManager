using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManager.Models;

namespace TaskManager.Services;

public interface ITaskService
{
    Task<List<TaskItem>> LoadTasksAsync();
    Task SaveTasksAsync(IEnumerable<TaskItem> tasks);
}
