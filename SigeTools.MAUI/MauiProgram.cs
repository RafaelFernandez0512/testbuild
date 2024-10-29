using CommunityToolkit.Maui;
using Controls.UserDialogs.Maui;
using Microsoft.Extensions.Logging;
using SigeTools.MAUI.Helpers;
using SigeTools.MAUI.Services;
using SigeTools.MAUI.ViewModels;
using SigeTools.MAUI.Views;
using SigeTools.ViewModel;
using Syncfusion.Maui.Core.Hosting;
using ZXing.Net.Maui.Controls;

namespace SigeTools.MAUI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            .UsePrism(PrismConfiguration.Initializer)
            .ConfigureSyncfusionCore()
            .UseUserDialogs()


            .UseBarcodeReader()
            .UseMauiCommunityToolkit();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }

    public static class PrismConfiguration
    {
        public static void Initializer(PrismAppBuilder builder)
        {
            builder.CreateWindow("/LoginPage", x =>
            {
                Console.WriteLine(x);
            });
            builder.OnInitialized((p) => { });
            builder.ConfigureServices(ConfigureNavigation);
            builder.ConfigureServices(ConfigureServices);
        }

        private static void ConfigureNavigation(IServiceCollection provider)
        {
            provider.RegisterForNavigation<StartUpPage, StartUpPageViewModel>();
            provider.RegisterForNavigation<MainPage, MainPageViewModel>();
            provider.RegisterForNavigation<TomaOrdenPage, TomaOrdenPageViewModel>();
            provider.RegisterForNavigation<ConsultaInventarioPage, ConsultaInventarioPageViewModel>();
            provider.RegisterForNavigation<InventariosPage, InventariosViewModel>();
            provider.RegisterForNavigation<LoginPage,LoginPageViewModel>();
            provider.RegisterForNavigation<ReportPage, ReportPageViewModel>();
            provider.RegisterForNavigation<TomaInventarioPage, TomaInventarioPageViewModel>();

        }

        private static void ConfigureServices(IServiceCollection provider)
        {
            provider.AddTransient<ISigeApi>((p) =>
                (new ApiService<ISigeApi>(AppSettings.BaseServiceUrl)).GetApi(Priority.UserInitiated));
            provider.AddSingleton<ISigeService, SigeService>();
            provider.AddSingleton<IUserDialogs>((p) => UserDialogs.Instance);
        }
    }
}