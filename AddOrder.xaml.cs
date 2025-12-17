using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Cashier;

namespace CashierExternal;

public partial class AddOrder : ContentPage
{
	ObservableCollection<Product> products = new ObservableCollection<Product>();
	public AddOrder()
	{
		InitializeComponent();
		LoadEmployees();
		products.CollectionChanged += (s, e) =>
		{
			ReloadTotal();
		};
		ProductListView.ItemsSource = products;
		//BarcodeEntry.Completed += (s, e) => AddProduct();
		BarcodeEntry.ReturnCommand = new Command(() => AddProduct());

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
			foreach (var x in products)
			{
				var price = double.Parse(x.quant) * x.ProudctPrice;
				total += price;
			}
			Total.Text = total.ToString();
		}
		catch (Exception ex)
		{

		}
	}
	public async void AddProduct()
	{
		var product = await GetProductInfo();
		products.Add(product);

	}
	public async Task<Product> GetProductInfo()
	{
		string url = "http://192.168.1.19:7084/api/GetProductInfo/";
		string barcodeNumber = BarcodeEntry.Text;
		using HttpClient client = new HttpClient();
		var json = JsonSerializer.Serialize(new BarcodeModel() { Barcode = barcodeNumber });
		var content = new StringContent(json, Encoding.UTF8, "application/json");

		HttpResponseMessage response = await client.PostAsync(url, content);
		// response.EnsureSuccessStatusCode();
		var st = await response.Content.ReadAsStringAsync();
		var product = JsonSerializer.Deserialize<Product>(st);
		return product;
	}
	public async Task<List<string>> GetEmployees()
	{
		string url = "http://192.168.1.19:7084/api/GetEmployees/";
		string barcodeNumber = BarcodeEntry.Text;
		using HttpClient client = new HttpClient();
		// var json = JsonSerializer.Serialize(new BarcodeModel() { Barcode = barcodeNumber });
		// var content = new StringContent(json, Encoding.UTF8, "application/json");

		HttpResponseMessage response = await client.PostAsync(url,null);
		// response.EnsureSuccessStatusCode();
		var st = await response.Content.ReadAsStringAsync();
		var product = JsonSerializer.Deserialize<List<string>>(st);
		return product;
	}
	public async Task AddOrderr(Order order)
	{
		string url = "http://192.168.1.19:7084/api/AddOrder/";

		using HttpClient client = new HttpClient();
		var json = JsonSerializer.Serialize(order);
		var content = new StringContent(json, Encoding.UTF8, "application/json");

		HttpResponseMessage response = await client.PostAsync(url, content);
		// response.EnsureSuccessStatusCode();
		// var st = await response.Content.ReadAsStringAsync();
		// var product = JsonSerializer.Deserialize<Product>(st);

	}

	private async void OnBarcodeScanClicked(object sender, EventArgs e)
	{
		var text = await BarcodePage.GetBarcode(this);
		BarcodeEntry.Text = text;
	}

	private void Delete_Clicked(object sender, EventArgs e)
	{
		var but = (sender as Button).BindingContext as Product;
		var list = products.Select(p => p.ProudctId).ToList();
		var ind = list.IndexOf(but.ProudctId);
		products.RemoveAt(ind);
	}

	private void Entry_TextChanged(object sender, TextChangedEventArgs e)
	{
		ReloadTotal();
	}

	private async void CreateBut_Clicked(object sender, EventArgs e)
	{
		var order = new Order()
		{
			OrderDate = DateTime.Now,
			OrderId = GenerateRandomFourDigitNumber().ToString(),
			Products = products.ToList(),
			Total = double.Parse(Total.Text),
			EmployeeName = EmployeePicker.SelectedItem.ToString() ?? "المحل",
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
	public int GenerateRandomFourDigitNumber()
{
    Random random = new Random();
    return random.Next(1000, 10000); // Generates a number between 1000 and 9999
}
}