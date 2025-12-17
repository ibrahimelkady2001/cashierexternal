using System.Threading.Tasks;
using Cashier;

namespace CashierExternal;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
	}


	private async void AddOrder_OnClicked(object? sender, EventArgs e)
	{
		await Navigation.PushAsync(new AddOrder());
	}


	private async void AddProduct_Clicked(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new AddProduct());
    }

	private async void ProductPrice_Clicked(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new ProductPricePage());
    }
}

