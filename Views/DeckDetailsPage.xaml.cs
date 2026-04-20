using Flashcard_Mobile.Services;

namespace Flashcard_Mobile.Views;

[QueryProperty(nameof(DeckId), "deckId")]
public partial class DeckDetailsPage : ContentPage
{
    private readonly DeckStore _deckStore = DeckStore.Instance;

    public string DeckId
    {
        set
        {
            if (!Guid.TryParse(value, out var id))
            {
                return;
            }

            var deck = _deckStore.GetById(id);
            if (deck is null)
            {
                DeckTitleLabel.Text = "Deck not found";
                FlashcardsCountLabel.Text = string.Empty;
                FlashcardsView.ItemsSource = null;
                return;
            }

            DeckTitleLabel.Text = deck.Title;
            FlashcardsCountLabel.Text = $"{deck.Flashcards.Count} flashcards";
            FlashcardsView.ItemsSource = deck.Flashcards;
        }
    }

    public DeckDetailsPage()
    {
        InitializeComponent();
    }
}
