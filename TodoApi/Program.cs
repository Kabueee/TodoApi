using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using TodoApi;
using TodoApi.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite("Data Source=tasks.db"));

builder.Services.AddScoped<ITaskRepository, EfTaskRepository>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/",  () => "Tasks in http://localhost:5087/tasks!");

app.MapGet("/tasks", async (ITaskRepository repo, int page = 1, int pageSize = 10, string sortBy = "createdAt",  string sortOrder = "asc", bool? isDone = null) =>
{
    var query = repo.GetAll();

    if (isDone.HasValue) query = query.Where(t => t.IsDone == isDone.Value);
    
    query = sortBy.ToLower() switch
    {
        "title" => sortOrder == "asc" ? query.OrderBy(t => t.Title) : query.OrderByDescending(t => t.Title),
        "isDone" => sortOrder == "asc" ? query.OrderBy(t => t.IsDone) : query.OrderByDescending(t => t.IsDone),
        _ => sortOrder == "asc" ? query.OrderBy(t => t.CreatedAt) : query.OrderByDescending(t => t.CreatedAt)
    };
    
    var totalCount = await query.CountAsync();
    
    var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
    
    var response = new PaginatedResponse<TaskItem>()
    {
        Items = items,
        TotalCount = totalCount,
        Page = page,
        PageSize = pageSize,
    };
    
    return Results.Ok(response);
});

app.MapGet("/tasks/search", async (string q,  ITaskRepository repo) =>
{
    var result = await repo.GetAll().Where(t => t.Title.ToLower().Contains(q.ToLower())).ToListAsync();
    return Results.Ok(result);
});

app.MapGet("/tasks/{id}", async (int id, ITaskRepository repo) =>
{
    var task = await repo.GetByIdAsync(id);
    return task is null ? Results.NotFound() : Results.Ok(task);
});

app.MapPost("/tasks", async (TaskItem request, ITaskRepository repo, ILogger<Program> logger) =>
{
    if (!request.IsValid) return Results.BadRequest("Invalid input");
    
    request.Id = 0;
    request.CreatedAt = DateTime.UtcNow;
    
    await repo.AddAsync(request);
    
    logger.LogInformation("New task: {Title}", request.Title);
    
    return Results.Created("", $"Task '{request.Title}' added");
});

app.MapPut("/tasks/{id}", async (int id, TaskItem request, ITaskRepository repo, ILogger<Program> logger) =>
{
    var task = await repo.GetByIdAsync(id);
    if (task == null) return Results.NotFound();
    
    task.Title = request.Title;
    task.IsDone = request.IsDone;

    await repo.UpdateAsync(task);
    
    logger.LogInformation("The task has been updated: {Title}", task.Title);
    return Results.Ok(task);
});

app.MapDelete("/tasks/{id}", async (int id, ITaskRepository repo, ILogger<Program> logger) =>
{
    var task = await repo.GetByIdAsync(id);
    if (task == null) return Results.NotFound();

    repo.DeleteAsync(id);
    
    logger.LogInformation("The task has been deleted: {Title}", task.Title);
    return Results.NoContent();
});

app.MapDelete("/tasks/all", async (ITaskRepository repo, ILogger<Program> logger) =>
{
    var count = await repo.CountAsync();
    if (count == 0) return Results.BadRequest("The task list is already empty");

    await repo.DeleteAllAsync();
    
    logger.LogInformation("All tasks have been deleted");
    return Results.Ok("All tasks have been deleted");
});

app.MapPatch("/tasks/{id}/status", async (int id, ITaskRepository repo, ILogger<Program> logger) =>
{
    var updated = await repo.ToggleStatusAsync(id);
    if(!updated) return Results.NotFound();
    logger.LogInformation($"Status toggled for task {id}");
    return Results.Ok(new { message = "Status toggled" });
});

app.Run();

async Task SaveTasksAsync(List<TaskItem> tasks)
{
    try
    {
        await using FileStream fs = new FileStream("tasks.json", FileMode.Create);
        await JsonSerializer.SerializeAsync(fs, tasks);
    }
    catch(Exception ex)
    {
        Console.WriteLine($"Save error: {ex.Message}");
    }
}

async Task<List<TaskItem>> LoadTasksAsync()
{
    const string path = "tasks.json";

    if (!File.Exists(path)) return new List<TaskItem>();
    
    try
    {
        await using FileStream fs = new FileStream(path, FileMode.Open);
            
        var tasks = await JsonSerializer.DeserializeAsync<List<TaskItem>>(fs);
        return tasks ?? new List<TaskItem>();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Save error: {ex.Message}");
        return new List<TaskItem>();
    }
}


