using Flashcard_Mobile.Models;
using Flashcard_Mobile.Services;

namespace Flashcard_Mobile.Views;

[QueryProperty(nameof(DeckId), "deckId")]
[QueryProperty(nameof(FlashcardId), "flashcardId")]
public partial class FlashcardFormPage : ContentPage
{
    private readonly DeckStore _deckStore = DeckStore.Instance;
    private Guid _deckId;
    private Flashcard? _currentFlashcard;

    public string DeckId
    {
        set
        {
            if (Guid.TryParse(value, out var id))
            {
                _deckId = id;
            }
        }
    }

    public string FlashcardId
    {
        set
        {
            if (Guid.TryParse(value, out var id))
            {
                var deck = _deckStore.GetById(_deckId);
                _currentFlashcard = deck?.Flashcards.FirstOrDefault(f => f.Id == id);
            }
            else
            {
                _currentFlashcard = null;
            }

            BindFlashcardToUi();
        }
    }

    public FlashcardFormPage()
    {
        InitializeComponent();
        BindFlashcardToUi();
    }

    private void BindFlashcardToUi()
    {
        if (_currentFlashcard is null)
        {
            TitleLabel.Text = "Create Flashcard";
            SaveButton.Text = "Create";
            DeleteButton.IsVisible = false;
            FrontEntry.Text = string.Empty;
            BackEntry.Text = string.Empty;
            return;
        }

        TitleLabel.Text = "Edit Flashcard";
        SaveButton.Text = "Update";
        DeleteButton.IsVisible = true;
        FrontEntry.Text = _currentFlashcard.Front;
        BackEntry.Text = _currentFlashcard.Back;
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        var front = FrontEntry.Text?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(front))
        {
            await DisplayAlert("Validation", "Front text is required.", "OK");
            return;
        }

        var back = BackEntry.Text?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(back))
        {
            await DisplayAlert("Validation", "Back text is required.", "OK");
            return;
        }

        if (_currentFlashcard is null)
        {
            _deckStore.AddFlashcard(_deckId, front, back);
        }
        else
        {
            _deckStore.UpdateFlashcard(_deckId, _currentFlashcard.Id, front, back);
        }

        await Shell.Current.GoToAsync("..");
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (_currentFlashcard is null)
            return;

        var shouldDelete = await DisplayAlert(
            "Delete flashcard",
            $"Delete this flashcard?",
            "Delete",
            "Cancel");

        if (!shouldDelete)
            return;

        _deckStore.DeleteFlashcard(_deckId, _currentFlashcard.Id);
        await Shell.Current.GoToAsync("..");
    }
}