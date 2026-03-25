namespace Flashcard_Mobile.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new Flashcard_Mobile.ViewModels.MainPageViewModel();
        }
    }
}
