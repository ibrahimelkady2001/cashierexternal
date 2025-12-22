using System;

namespace Cashier;

public class Product
{
    public string id { get; set; }
public string ProudctId { get; set; }
public string ProudctName { get; set; }
public double ProudctPrice { get; set; }
public double RealPrice { get; set; }

public double GomlaPrice { get; set; }
public string SupplierName { get; set; }
public double Quantity { get; set; }

public int SubProductCount { get; set; }
public string SubProducttype { get; set; }
public Product SubProduct { get; set; }
    public string _quant = "1";
public string quant { get
        {
            return _quant;
} set
        {
            _quant = value;
} }
        
}

public class BarcodeModel
{
    public string Barcode { get; set; }
}
