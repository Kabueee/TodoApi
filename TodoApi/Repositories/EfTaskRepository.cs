using Microsoft.EntityFrameworkCore;

namespace TodoApi.Repositories;

public class EfTaskRepository : ITaskRepository
{
    private readonly AppDbContext _context;

    public EfTaskRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public IQueryable<TaskItem> GetAll()
    {
        return _context.TaskItems.AsQueryable();
    }

    public async Task<TaskItem?> GetByIdAsync(int id)
    {
        return await _context.TaskItems.FindAsync(id);
    }

    public async Task AddAsync(TaskItem task)
    {
        await _context.TaskItems.AddAsync(task);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(TaskItem task)
    {
        _context.TaskItems.Update(task);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var task = await GetByIdAsync(id);
        if (task != null)
        {
            _context.TaskItems.Remove(task);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteAllAsync()
    {
        await _context.TaskItems.ExecuteDeleteAsync();
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.TaskItems.AnyAsync(t => t.Id == id);
    }

    public async Task<int> CountAsync()
    {
        return await  _context.TaskItems.CountAsync();
    }

    public async Task<bool> ToggleStatusAsync(int id)
    {
        var task = await _context.TaskItems.FindAsync(id);
        if (task == null) return false;
        
        task.IsDone = !task.IsDone;
        await _context.SaveChangesAsync();
        return true;
    }
}