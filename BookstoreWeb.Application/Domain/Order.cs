namespace BookstoreWeb.Application.Domain;

public class Order
{
    public int      OrderID       { get; set; }
    public string   UserID        { get; set; } = string.Empty;
    public string   Status        { get; set; } = string.Empty;
    public DateTime? OrderDate    { get; set; }
    public decimal?  TotalAmount  { get; set; }

    // Filled during order confirmation
    public string? FullName       { get; set; }
    public string? Email          { get; set; }
    public string? Phone          { get; set; }
    public string? Address        { get; set; }
    public string? Note           { get; set; }
    public string? PaymentMethod  { get; set; }

    // Navigation property
    public List<OrderDetail>? OrderDetails { get; set; }
}
