using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SampleCGFSMobileApp;

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
            });

        builder.Services.AddSingleton<HttpClient>(sp => new HttpClient());

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // Ensure default preferences are set at app startup
        InitializePreferences();

        return builder.Build();
    }

    private static void InitializePreferences()
    {
        // Only set defaults if they haven't been set before
        //if (!Preferences.ContainsKey("GUID"))
        //    Preferences.Set("GUID", string.Empty); // Default: No GUID (unregistered)

        //if (!Preferences.ContainsKey("IsHeadOfHousehold"))
        //    Preferences.Set("IsHeadOfHousehold", false); // Default: Not Head of Household

        //if (!Preferences.ContainsKey("Birthdate"))
        //    Preferences.Set("Birthdate", "01/01/2005"); // Default: Under 18 for testing
    }
}
