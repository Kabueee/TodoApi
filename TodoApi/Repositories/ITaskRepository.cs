namespace TodoApi.Repositories;

public interface ITaskRepository
{
    IQueryable<TaskItem> GetAll();
    Task<TaskItem?> GetByIdAsync(int id);
    Task AddAsync(TaskItem task);
    Task UpdateAsync(TaskItem task);
    Task DeleteAsync(int id);
    Task DeleteAllAsync();
    Task<bool> ExistsAsync(int id);
    Task<int> CountAsync();
    Task<bool> ToggleStatusAsync(int id);
}