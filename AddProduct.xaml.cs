using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CashierExternal;


namespace Cashier;

public partial class AddProduct : ContentPage
{
	public  Product product = new Product();
	bool IsProductDetail = false;
	public AddProduct(Product product = null)
	{
		InitializeComponent();
		if (product != null)
		{
			addproductbut.Text = "تعديل المنتج";
			IsProductDetail = true;
			ProductNameEntry.Text = product.ProudctName ?? string.Empty;
			RealPriceEntry.Text = product.RealPrice.ToString() ?? string.Empty;
			ProductQuantaty.Text = product.Quantity.ToString() ?? string.Empty;
			if (product.SubProduct != null)
			{
				subcheckbox.IsChecked = true;
				subname.Text = product.SubProduct.ProudctName;
				SubProductent.Text = product.SubProduct.ProudctName ?? string.Empty;
				SubProductCountent.Text = product.SubProductCount.ToString() ?? string.Empty;
				var total = product.Quantity;
				double integerPart = Math.Floor(total); // Get the integer part
				double decimalPart = total - integerPart; // Calculate the decimal part
				ProductQuantaty.Text = integerPart.ToString();
				SubProuctQuantatity.Text = Math.Round(decimalPart * product.SubProductCount).ToString();
				SubProuctQuantatity.IsVisible = true;
					subpriceent.Text = product.SubProduct.ProudctPrice.ToString() ?? string.Empty;
		//	SubProuctQuantatity.Text = product.SubProductCount.ToString() ?? string.Empty;
			}
			
			gomlapriceentry.Text = product.GomlaPrice.ToString() ?? string.Empty;
		
		
			PriceEntry.Text = product.ProudctPrice.ToString() ?? string.Empty;
		//	SubProuctQuantatity.Text = product.SubProductCount.ToString() ?? string.Empty;
			// ProductQuantityEntry.Text = product.Quantity.ToString() ?? string.Empty;
			BarcodeEntry.Text = product.ProudctId ?? string.Empty;
		}

	}
	public static string GenerateRandom8DigitNumber()
	{
	Random random = new Random();
        string result = random.Next(100, 999).ToString(); // First 3 digits (non-zero)

        for (int i = 0; i < 9; i++) // Remaining 9 digits
        {
            result += random.Next(0, 10).ToString();
        }

        return result;
	}
	private async void AddProduct_Clicked(System.Object sender, System.EventArgs e)
	{

		var product = new Product()
		{
			ProudctId = BarcodeEntry.Text,
			ProudctName = ProductNameEntry.Text,
			// SubProduct = SubProductent.Text,

			// ProudctPrice = PriceEntry.Text
			// Quantity = ProductQuantityEntry.Text,
		};
		if (subcheckbox.IsChecked)
		{
var SubProduct = new Product(){ProudctId = SubProductent.Text, ProudctPrice = double.Parse(subpriceent.Text),ProudctName = subname.Text };
			var total = double.Parse(ProductQuantaty.Text) + ((double.Parse(SubProuctQuantatity.Text ?? "0") / double.Parse(SubProductCountent.Text ?? "1")));
			product.Quantity = total;
			product.SubProduct = SubProduct;
			product.SubProducttype = producttypeent.Text;
			product.SubProductCount = int.Parse(SubProductCountent.Text);
		
		}
		else
		{
			product.Quantity = double.Parse(ProductQuantaty.Text);
		}
		bool allowed = true;
		if (double.TryParse(PriceEntry.Text, out double price))
		{
			product.ProudctPrice = price;
		}
		else
		{

			await DisplayAlert("معذرة", "قم بادخال سعر صحيح", "اغلاق");
			allowed = false;
		}
		if (double.TryParse(RealPriceEntry.Text, out double Realprice))
		{
			product.RealPrice = Realprice;
		}
		else
		{

			await DisplayAlert("معذرة", "قم بادخال سعر صحيح", "اغلاق");
			allowed = false;
		}
		if (double.TryParse(RealPriceEntry.Text, out double Gomlaprice))
		{
			product.GomlaPrice = Gomlaprice;
		}
		else
		{

			await DisplayAlert("معذرة", "قم بادخال سعر الجملة صحيح", "اغلاق");
			allowed = false;
		}
		// if (double.TryParse(ProductQuantaty.Text, out double Quantity))
		// {
		// 	product.Quantity = Quantity;
		// }
		// else
		// {

		// 	await DisplayAlert("معذرة", "قم بادخال كمية صحيحة", "اغلاق");
		// 	allowed = false;
		// }
		// if (int.TryParse(SubProductent.Text, out int SubProductCount))
		// {
		// 	product.SubProductCount = SubProductCount;
		// }
		// else
		// {

		// 	await DisplayAlert("معذرة", "قم بادخال سعر الجملة صحيح", "اغلاق");
		// 	allowed = false;
		// }
		if (allowed)
		{
			await AddProductt(product);
			await Navigation.PopAsync();
		}


		// await Navigation.PushAsync(new AddProduct());
	}
	public async Task AddProductt(Product order)
	{
	
		var json = JsonSerializer.Serialize(order);


		HttpResponseMessage response = await ApiWrapper.Post(json, "AddProduct");
		// response.EnsureSuccessStatusCode();
		// var st = await response.Content.ReadAsStringAsync();
		// var product = JsonSerializer.Deserialize<Product>(st);

	}
	public async Task print(Dictionary<string,string> order)
	{
	
		var json = JsonSerializer.Serialize(order);
		

		HttpResponseMessage response = await ApiWrapper.Post(json, "print");
		// response.EnsureSuccessStatusCode();
		// var st = await response.Content.ReadAsStringAsync();
		// var product = JsonSerializer.Deserialize<Product>(st);

	}


	private async void GenerateRandomBarcode_Clicked(object sender, EventArgs e)
	{

		BarcodeEntry.Text = GenerateRandom8DigitNumber();

	
	}
	private void GenerateRandoSubBarcode_Clicked(object sender, EventArgs e)
	{

		SubProductent.Text = GenerateRandom8DigitNumber();
	}
	private async void PrintSticker_Clicked(object sender, EventArgs e)
	{

	}

	private void subcheckbox_CheckedChanged(object sender, CheckedChangedEventArgs e)
	{
		SubProcutIsVis.IsVisible = e.Value;
		SubProuctQuantatity.IsVisible = e.Value;
	}

	private void producttypeent_TextChanged(object sender, TextChangedEventArgs e)
	{
		SubProuctQuantatity.Placeholder = e.NewTextValue;
	}


	private async void Print_Clicked(object sender, EventArgs e)
	{
		var name = SubProductent.Text;
		string result = "";
		  await MainThread.InvokeOnMainThreadAsync(async () =>
	  {
		  result = await DisplayPromptAsync("Required", "Quantity");

	  });
		var dic = new Dictionary<string, string>();
		dic.Add("barcode", name);
		dic.Add("quantity", result);
	await	print(dic);
	}
	private async void PrintMain_Clicked(object sender, EventArgs e)
	{
		var name = BarcodeEntry.Text;
		string result = "";
		  await MainThread.InvokeOnMainThreadAsync(async () =>
	  {
		  result = await DisplayPromptAsync("Required", "Quantity");

	  });
		var dic = new Dictionary<string, string>();
		dic.Add("barcode", name);
		dic.Add("quantity", result);
	await	print(dic);

    }

    private void ImageButton_Clicked(object sender, EventArgs e)
    {
    }

	private async void BarcodeCamera_Clicked(object sender, EventArgs e)
	{
		   #if IOS || ANDROID
			var text = await BarcodePage.GetBarcode(this);
		BarcodeEntry.Text = text;
		#endif
    }

	private async void BarcodeSubCamera_Clicked(object sender, EventArgs e)
	{
		   #if IOS || ANDROID
		var text = await BarcodePage.GetBarcode(this);
		SubProductent.Text = text;
		#endif
    }
		
	}
