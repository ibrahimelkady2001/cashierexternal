using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Cashier;

namespace CashierExternal;

public partial class ProductPricePage : ContentPage
{
	public ProductPricePage()
	{
		InitializeComponent();
	}

	private async void OnBarcodeScanClicked(object sender, EventArgs e)
	{
		var text = await BarcodePage.GetBarcode(this);
		BarcodeEntry.Text = text;
	}

	private async void OnSubmitClicked(object sender, EventArgs e)
	{
		try
		{

			var product = await GetProductInfo();
			
			var pg = new ContentPage();
			var stack = new StackLayout();
			var label = new Label()
			{
				Text = "اسم المنتج: " + product.ProudctName + "\nسعر المنتج: " + product.ProudctPrice,
				FontSize = 50,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				HorizontalOptions = LayoutOptions.CenterAndExpand

			};

			stack.Children.Add(label);
			pg.Content = stack;
			await Navigation.PushAsync(pg);
		
		}
		catch (Exception ex)
		{
			
		}
	}
	    public  async Task<Product> GetProductInfo( )
    {
		 string url = "http://192.168.1.19:7084/api/GetProductInfo/";
		string barcodeNumber = BarcodeEntry.Text;
        using HttpClient client = new HttpClient();
        var json = JsonSerializer.Serialize(new BarcodeModel(){Barcode = barcodeNumber});
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await client.PostAsync(url, content);
		// response.EnsureSuccessStatusCode();
		var st = await response.Content.ReadAsStringAsync();
var product = JsonSerializer.Deserialize<Product>(st);
		return product;
    }
}