using Microsoft.Extensions.Logging;
using Flashcard_Mobile.Views;

namespace Flashcard_Mobile
{
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

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            // Register routes for pages not in Shell
            Routing.RegisterRoute(nameof(DeckFormPage), typeof(DeckFormPage));
            Routing.RegisterRoute(nameof(FlashcardFormPage), typeof(FlashcardFormPage));

            return builder.Build();
        }
    }
}
