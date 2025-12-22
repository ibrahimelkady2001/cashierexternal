using Syncfusion.Licensing;

namespace CashierExternal;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        SyncfusionLicenseProvider.RegisterLicense(
                "Ngo9BigBOggjHTQxAR8/V1JFaF5cXGRCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWH9dcHZVRWddUkF0W0BWYEg=");
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }
}