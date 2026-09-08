namespace TodoApi;

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsDone { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public bool IsValid => !string.IsNullOrWhiteSpace(Title);
    
}