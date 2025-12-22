   #if IOS || ANDROID

using ZXing.Net.Maui.Controls;

namespace CashierExternal;

public class BarcodePage : ContentPage
{
public static	TaskCompletionSource<string> Barcode = new TaskCompletionSource<string>();
	public BarcodePage()
	{
		Barcode = new TaskCompletionSource<string>();
		var closebutton = new Button
		{
			Text = "اغلاق",
			HorizontalOptions = LayoutOptions.EndAndExpand,
			FontSize = 30,
			TextColor = Colors.Yellow,
			BackgroundColor = Colors.Red
		};
	
		var barcodeview = new BarcodeScanner.Mobile.CameraView(){ VerticalOptions = LayoutOptions.FillAndExpand, HorizontalOptions = LayoutOptions.FillAndExpand, VibrationOnDetected = true,  };
		var stack = new StackLayout(){VerticalOptions= LayoutOptions.FillAndExpand, Children = { barcodeview, closebutton } };
	
		closebutton.Clicked += async (sender, e) =>
		{
await Navigation.PopModalAsync();
		};

		
		Content = stack;
		barcodeview.OnDetected+= (sender, e) =>
		{
			
			Barcode.TrySetResult(e.BarcodeResults.FirstOrDefault().RawValue);
		};
	}
	public static async Task<string> GetBarcode(Page pg)
	{
		await pg.Navigation.PushModalAsync(new NavigationPage(new  BarcodePage()));
		var text = await Barcode.Task;
		await pg.Navigation.PopModalAsync();
		return text;
	}
}
#endif