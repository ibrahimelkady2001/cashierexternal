using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Cashier;

namespace CashierExternal;

public partial class AddOrder : ContentPage
{
	ObservableCollection<Product> products = new ObservableCollection<Product>();
		List<Product> Allproducts = new List<Product>();
	public AddOrder()
	{
try
{
			InitializeComponent();
			LoadProducts();
		//	LoadEmployees();
			products.CollectionChanged += (s, e) =>
			{
				ReloadTotal();
			};
			ProductListView.ItemsSource = products;
			//BarcodeEntry.Completed += (s, e) => AddProduct();
			BarcodeEntry.ReturnCommand = new Command(() => AddProduct());
}
catch (System.Exception ex)
{

			Debug.WriteLine(ex);
}

	}
	public async void LoadProducts()
	{
		try
		{
			
			Allproducts = await GetAllProducts();
			BarcodeEntry.ItemsSource = Allproducts;
			BarcodeEntry.PropertyChanged += (s, p) =>
			{
	
				if (p.PropertyName == "Text")
				{
					if (string.IsNullOrEmpty(BarcodeEntry.Text))
					{
						BarcodeEntry.ItemsSource = Allproducts;
	
					}
					else
					{
						BarcodeEntry.ItemsSource = Allproducts
			.Where(p =>
				(p.ProudctId?.Contains(BarcodeEntry.Text) ?? false) ||
				(p.ProudctName?.Contains(BarcodeEntry.Text) ?? false))
			.ToList();
					}
				}
	
			};
	 
		}
		catch (System.Exception ex)
		{
			Debug.WriteLine(ex);
		
		}
    }
	public async void LoadEmployees()
	{
		var Employees = await GetEmployees();
		Employees.Insert(0, "المحل");

		EmployeePicker.ItemsSource = Employees;
		EmployeePicker.SelectedIndex = 0;
	}
	public void ReloadTotal()
	{
		try
		{


			double total = 0;
			foreach (var x in  products)
			{
				var val = double.TryParse(x.quant,out double value);
				if (val)
                {
                    
               
				var price =value  * x.ProudctPrice;
				total += price;
				 }
                else
                {
                    	var price = x.ProudctPrice;
				total += price;
                }
			}
			Total.Text = total.ToString();
		}
		catch (Exception ex)
		{

		}
	}
	public async void AddProduct()
	{
		var text = BarcodeEntry.Text;
		var product = Allproducts.Find(p => p.ProudctName == text || p.ProudctId == text);
	if (product != null)
        {
             products.Add(product);
        }
		// var product = await GetProductInfo();
		

	}
	public async Task<List<Product>> GetAllProducts()
	{




		HttpResponseMessage response = await ApiWrapper.Post(null, "GetProducts");
		// response.EnsureSuccessStatusCode();
		if (response.IsSuccessStatusCode) {
			var st = await response.Content.ReadAsStringAsync();
			// var doc = JsonDocument.Parse(st);
		
			var product = JsonSerializer.Deserialize<List<Product>>( st);
		return product;
	}
		return new ();
	}
	public async Task<List<string>> GetEmployees()
	{

		// var json = JsonSerializer.Serialize(new BarcodeModel() { Barcode = barcodeNumber });
		// var content = new StringContent(json, Encoding.UTF8, "application/json");

		HttpResponseMessage response = await ApiWrapper.Post(null, "GetEmployees");
		// response.EnsureSuccessStatusCode();
	
		var st = await response.Content.ReadAsStringAsync();
		var product = JsonSerializer.Deserialize<List<string>>(st);
		return product;
	}
	public async Task AddOrderr(Order order)
	{
	
		var json = JsonSerializer.Serialize(order);
		

		HttpResponseMessage response = await ApiWrapper.Post(json, "AddOrder");
		// response.EnsureSuccessStatusCode();
		// var st = await response.Content.ReadAsStringAsync();
		// var product = JsonSerializer.Deserialize<Product>(st);

	}

	private async void OnBarcodeScanClicked(object sender, EventArgs e)
	{
		   #if IOS || ANDROID
		var text = await BarcodePage.GetBarcode(this);
		BarcodeEntry.Text = text;
		#endif
	}

	private void Delete_Clicked(object sender, EventArgs e)
	{
		var but = (sender as Button).BindingContext as Product;
		var list =  products.Where(p => !(p.ProudctId == but.ProudctId || p.ProudctName == but.ProudctName)).ToList();
		 products.Clear();

		foreach(var product in list)
        {
            products.Add(product);
        }
	}

	private void Entry_TextChanged(object sender, TextChangedEventArgs e)
	{

	//	
	}
	private async void BarcodeEntry_SelectionChanged(object sender, EventArgs e)
    {
		if (BarcodeEntry.SelectedItem is Product product)
        {
            products.Add( product);
     
		ReloadTotal();
		
		   }
    }
	private async void CreateBut_Clicked(object sender, EventArgs e)
	{
	try
	{
		Debug.WriteLine(1222);
			var order = new Order()
			{
				OrderDate = DateTime.Now,
				OrderId = GenerateRandomFourDigitNumber().ToString(),
				Products = products.ToList(),
				Total = double.Parse(Total.Text),
			
			};
			await AddOrderr(order);
	var pg = new ContentPage();
	var stack = new StackLayout();
			var label = new Label()
			{
				Text = $"رقم طلبك:{order.OrderId}",
				FontSize = 50,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				HorizontalOptions = LayoutOptions.CenterAndExpand
	
			};
	stack.Children.Add(label);
	pg.Content = stack;
	await Navigation.PushAsync(pg);
	}
	catch (System.Exception ex)
	{
		
	Debug.WriteLine(ex);
	}
	}
	public int GenerateRandomFourDigitNumber()
{
    Random random = new Random();
    return random.Next(1000, 10000); // Generates a number between 1000 and 9999
}

    private async void Button_Clicked(object sender, EventArgs e)
	{
		var popup = new ProductsPicker();
		var product = await popup.ShowPopup(this, Allproducts.Where(p=>string.IsNullOrEmpty(p.ProudctId)).ToList());
if (product != null)
        {
			products.Add(product);
        }
    }
}