namespace Flashcard_Mobile
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(Views.DeckFormPage), typeof(Views.DeckFormPage));
        }

    }
}
