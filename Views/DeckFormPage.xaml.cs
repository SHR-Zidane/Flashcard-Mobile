using Flashcard_Mobile.Models;
using Flashcard_Mobile.Services;

namespace Flashcard_Mobile.Views;

[QueryProperty(nameof(DeckId), "deckId")]
public partial class DeckFormPage : ContentPage
{
    private readonly DeckStore _deckStore = DeckStore.Instance;
    private Deck? _currentDeck;

    public string DeckId
    {
        set
        {
            if (Guid.TryParse(value, out var id))
            {
                _currentDeck = _deckStore.GetById(id);
            }
            else
            {
                _currentDeck = null;
            }

            BindDeckToUi();
        }
    }

    public DeckFormPage()
    {
        InitializeComponent();
        BindDeckToUi();
    }

    private void BindDeckToUi()
    {
        if (_currentDeck is null)
        {
            TitleLabel.Text = "Create Deck";
            SaveButton.Text = "Create";
            DeleteButton.IsVisible = false;
            NameEntry.Text = string.Empty;
            ListEntry.Text = string.Empty;
            WordsCountEntry.Text = "0";
            return;
        }

        TitleLabel.Text = "Edit Deck";
        SaveButton.Text = "Update";
        DeleteButton.IsVisible = true;
        NameEntry.Text = _currentDeck.Title;
        ListEntry.Text = _currentDeck.ListName;
        WordsCountEntry.Text = _currentDeck.WordsCount.ToString();
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        var title = NameEntry.Text?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(title))
        {
            await DisplayAlert("Validation", "Deck name is required.", "OK");
            return;
        }

        var listName = ListEntry.Text?.Trim() ?? string.Empty;
        var parsedCount = int.TryParse(WordsCountEntry.Text, out var count) ? Math.Max(0, count) : 0;

        if (_currentDeck is null)
        {
            _deckStore.Create(title, listName, parsedCount);
        }
        else
        {
            _deckStore.Update(_currentDeck.Id, title, listName, parsedCount);
        }

        await Shell.Current.GoToAsync("..");
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (_currentDeck is null)
            return;

        var shouldDelete = await DisplayAlert(
            "Delete deck",
            $"Delete '{_currentDeck.Title}'?",
            "Delete",
            "Cancel");

        if (!shouldDelete)
            return;

        _deckStore.Delete(_currentDeck.Id);
        await Shell.Current.GoToAsync("..");
    }
}
