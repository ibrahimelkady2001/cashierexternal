
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Core.Hosting;
     #if IOS || ANDROID
using ZXing.Net.Maui.Controls;
using BarcodeScanner.Mobile;
#endif
namespace CashierExternal;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>().ConfigureSyncfusionCore() .UseMauiCommunityToolkit()
            #if IOS || ANDROID
            .UseBarcodeReader()    .ConfigureMauiHandlers(handlers =>
                {
                    handlers.AddBarcodeScannerHandler();
                })
                #endif
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });



        return builder.Build();
    }
}