namespace ECS_Logistics.DTOs;

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; set; }
    public int CurrentPage { get; set; }
    public int Offset { get; set; }
}