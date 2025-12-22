using System.Threading.Tasks;
using Cashier;
using CommunityToolkit.Maui.Extensions;

namespace CashierExternal;

public partial class ProductsPicker : ContentView
{
	TaskCompletionSource<Product> tcs = new();
	public ProductsPicker()
	{
		InitializeComponent();
		tcs = new();
	}
	public async Task<Product> ShowPopup(Page pg, List<Product> products)
	{
		
		pg.ShowPopup(this);
		col.ItemsSource = products;

		var product = await tcs.Task;
		await pg.ClosePopupAsync();
		return product;
    }

	private void close_Clicked(object sender, EventArgs e)
	{
		tcs.TrySetResult(null);
	}
	  private void Product_Clicked(object sender, EventArgs e)
	{
		var product = (sender as Button).BindingContext as Product;
		tcs.TrySetResult(product);
    }
}