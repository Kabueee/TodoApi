using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite("Data Source=tasks.db"));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/",  () => "Tasks in http://localhost:5087/tasks!");

app.MapGet("/tasks", async (AppDbContext db, int page = 1, int pageSize = 10, string sortBy = "createdAt",  string sortOrder = "asc", bool? isDone = null) =>
{
    var query = db.TaskItems.AsQueryable();

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

app.MapGet("/tasks/search", async (string q,  AppDbContext db) =>
{
    var result = await db.TaskItems.Where(t => EF.Functions.Like(t.Title, $"%{q}%")).ToListAsync();
    return Results.Ok(result);
});

app.MapGet("/tasks/{id}", async (int id, AppDbContext db) =>
{
    var task = await db.TaskItems.FindAsync(id);
    return task is null ? Results.NotFound() : Results.Ok(task);
});

app.MapPost("/tasks", async (TaskItem request, AppDbContext db, ILogger<Program> logger) =>
{
    if (!request.IsValid) return Results.BadRequest("Invalid input");
    
    request.Id = 0;
    request.CreatedAt = DateTime.UtcNow;
    
    await db.TaskItems.AddAsync(request);
    await db.SaveChangesAsync();
    
    logger.LogInformation("New task: {Title}", request.Title);
    
    return Results.Created("", $"Task '{request.Title}' added");
});

app.MapPut("/tasks/{id}", async (int id, TaskItem request, AppDbContext db, ILogger<Program> logger) =>
{
    var task = await db.TaskItems.FindAsync(id); 
    if (task == null) return Results.NotFound();
    
    task.Title = request.Title;
    task.IsDone = request.IsDone;
    
    await  db.SaveChangesAsync();
    
    logger.LogInformation("The task has been updated: {Title}", task.Title);
    return Results.Ok(task);
});

app.MapDelete("/tasks/{id}", async (int id, AppDbContext db, ILogger<Program> logger) =>
{
    var task = await db.TaskItems.FindAsync(id);
    if (task == null) return Results.NotFound();
    
    db.TaskItems.Remove(task);
    await  db.SaveChangesAsync();
    
    logger.LogInformation("The task has been deleted: {Title}", task.Title);
    return Results.NoContent();
});

app.MapDelete("/tasks/all", async (AppDbContext db, ILogger<Program> logger) =>
{
    var taskItems = await db.TaskItems.ToListAsync();
    if (taskItems.Count == 0) return Results.BadRequest("The task list is already empty");
    
    db.TaskItems.RemoveRange(taskItems);
    await db.SaveChangesAsync();
    
    logger.LogInformation("All tasks have been deleted");
    return Results.Ok("All tasks have been deleted");
});

app.MapPatch("/tasks/{id}/status", async (int id, AppDbContext db, ILogger<Program> logger) =>
{
    var task = await db.TaskItems.FindAsync(id);
    if (task == null) return Results.NotFound();

    task.IsDone = !task.IsDone;
    
    await db.SaveChangesAsync();
    logger.LogInformation($"The status of task '{task.Title}' has been changed to {task.IsDone}");
    return Results.Ok(task);
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


