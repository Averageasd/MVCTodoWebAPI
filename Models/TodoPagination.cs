namespace TodoProjectSample.Models
{
    public class TodoPagination
    {
        public int Page { get; set; }
        public readonly int NUM_VISIBLE_ITEMS = 5;
    }
}
