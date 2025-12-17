using System;

namespace Cashier;

public class Order
{
public string OrderId { get; set; }
public DateTime OrderDate { get; set; }
public List<Product> Products { get; set; }
public double Total { get; set; }
public string EmployeeName { get; set; }
}
