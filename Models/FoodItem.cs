namespace KomodoDesktop.Models;

public class FoodItem
{
    public int     Id       { get; set; }
    public string  FoodName { get; set; } = "";
    public decimal Price    { get; set; }
    public string  Category { get; set; } = "";
}
