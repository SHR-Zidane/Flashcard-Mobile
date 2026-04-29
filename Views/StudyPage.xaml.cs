using Flashcard_Mobile.Services;
using Flashcard_Mobile.ViewModels;

namespace Flashcard_Mobile.Views;

[QueryProperty(nameof(DeckId), "deckId")]
public partial class StudyPage : ContentPage
{
    private readonly DeckStore _deckStore = DeckStore.Instance;
    private readonly StudyViewModel _viewModel;

    public string DeckId
    {
        set
        {
            if (!Guid.TryParse(value, out var id))
            {
                return;
            }

            var deck = _deckStore.GetById(id);
            if (deck == null)
            {
                DisplayAlert("Erreur", "Deck introuvable", "OK");
                Shell.Current.GoToAsync("..");
                return;
            }

            if (deck.Flashcards.Count == 0)
            {
                DisplayAlert("Impossible", "Aucune flashcard dans ce deck. Ajoutez des flashcards avant d'étudier.", "OK");
                return;
            }

            _viewModel.StartStudy(deck);
        }
    }

    public StudyPage()
    {
        InitializeComponent();
        _viewModel = new StudyViewModel();
        BindingContext = _viewModel;
    }
}