using Flashcard_Mobile.ViewModels;

namespace Flashcard_Mobile.Views;

public partial class SearchPage : ContentPage
{
    private readonly SearchViewModel _viewModel;

    public SearchPage()
    {
        InitializeComponent();
        _viewModel = new SearchViewModel();
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Refresh search results when returning to the page
        _viewModel.SearchText = _viewModel.SearchText;
    }
}

