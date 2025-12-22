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
		#if ANDROID || IOS
		var text = await BarcodePage.GetBarcode(this);
		BarcodeEntry.Text = text;
		#endif
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
public async Task<Product?> GetProductInfo()
{
    string barcodeNumber = BarcodeEntry.Text;
    using HttpClient client = new HttpClient();

    var json = JsonSerializer.Serialize(new BarcodeModel { Barcode = barcodeNumber });


    // Common LAN IP prefix (192.168.1.x) — adjust if your network uses 10.x.x.x or 192.168.0.x


    // Try all possible IPs on the subnet (e.g. 192.168.1.1 to 192.168.1.255)
 
                HttpResponseMessage response = await ApiWrapper.Post(json, "GetProductInfo");

            if (response.IsSuccessStatusCode)
            {
                string st = await response.Content.ReadAsStringAsync();
                var product = JsonSerializer.Deserialize<Product>(st);
                if (product != null)
                {

                    return product;
                }
            }
     

    System.Diagnostics.Debug.WriteLine("❌ Could not reach API on any local IP.");
    return null;
}
}