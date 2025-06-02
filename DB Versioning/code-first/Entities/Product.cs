namespace code_first.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public bool IsDeleted { get; set; }
    public Category Category { get; set; } = null!;
}
