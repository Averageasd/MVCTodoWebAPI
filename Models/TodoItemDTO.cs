namespace TodoProjectSample.Models
{
    /// <summary>
    /// DTO for TodoItem model
    /// contains fields that are visible to clients
    /// does not contain fields that clients are not supposed to see
    /// </summary>
    public class TodoItemDTO
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public bool IsComplete { get; set; }
    }
}
