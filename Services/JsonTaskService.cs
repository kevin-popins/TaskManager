using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TaskManager.Models;

namespace TaskManager.Services;

public class JsonTaskService : ITaskService
{
    private static readonly string FilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "TaskManager", "tasks.json");

    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public async Task<List<TaskItem>> LoadTasksAsync()
    {
        if (!File.Exists(FilePath)) return [];
        var json = await File.ReadAllTextAsync(FilePath);
        return JsonSerializer.Deserialize<List<TaskItem>>(json, Options) ?? [];
    }

    public async Task SaveTasksAsync(IEnumerable<TaskItem> tasks)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
        var json = JsonSerializer.Serialize(tasks, Options);
        await File.WriteAllTextAsync(FilePath, json);
    }
}
