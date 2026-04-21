using Flashcard_Mobile.Services;
using Flashcard_Mobile.ViewModels;
using Flashcard_Mobile.Models;

namespace Flashcard_Mobile.Views;

[QueryProperty(nameof(DeckId), "deckId")]
public partial class DeckDetailsPage : ContentPage
{
    private readonly DeckStore _deckStore = DeckStore.Instance;
    private readonly DeckDetailsViewModel _viewModel;
    private Guid _deckId;

    public string DeckId
    {
        set
        {
            if (!Guid.TryParse(value, out var id))
            {
                _deckId = Guid.Empty;
                return;
            }

            _deckId = id;
            var deck = _deckStore.GetById(id);
            if (deck is null)
            {
                DeckTitleLabel.Text = "Deck not found";
                _viewModel.SetDeck(new Deck()); // empty
                return;
            }

            DeckTitleLabel.Text = deck.Title;
            _viewModel.SetDeck(deck);
        }
    }

    public DeckDetailsPage()
    {
        InitializeComponent();
        _viewModel = new DeckDetailsViewModel();
        BindingContext = _viewModel;
        FlashcardsView.ItemsSource = _viewModel.Flashcards;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (_deckId != Guid.Empty)
        {
            var deck = _deckStore.GetById(_deckId);
            if (deck != null)
            {
                _viewModel.SetDeck(deck);
            }
        }
    }
}
