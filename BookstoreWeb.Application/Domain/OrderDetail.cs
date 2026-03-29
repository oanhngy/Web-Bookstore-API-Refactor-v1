namespace BookstoreWeb.Application.Domain;

public class OrderDetail
{
    public int     OrderDetailID { get; set; }
    public int     OrderID       { get; set; }
    public int     ProductID     { get; set; }
    public int     Quantity      { get; set; }

    // Snapshot of price at time of purchase — not linked to current Product.Price
    public decimal UnitPrice     { get; set; }

    // Navigation properties
    public Order?   Order   { get; set; }
    public Product? Product { get; set; }
}
