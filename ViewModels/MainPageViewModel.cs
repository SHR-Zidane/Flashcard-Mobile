using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Flashcard_Mobile.Models;
using Flashcard_Mobile.Services;
using Flashcard_Mobile.Views;

namespace Flashcard_Mobile.ViewModels;

public class MainPageViewModel
{
    private readonly DeckStore _deckStore = DeckStore.Instance;
    public ObservableCollection<Deck> Decks => _deckStore.Decks;

    public ICommand ModifyDeckCommand { get; }
    public ICommand DeleteDeckCommand { get; }
    public ICommand AddDeckCommand { get; }

    public MainPageViewModel()
    {
        AddDeckCommand = new Command(async () =>
        {
            await Shell.Current.GoToAsync(nameof(DeckFormPage));
        });

        ModifyDeckCommand = new Command<Deck>(async deck =>
        {
            if (deck is null)
                return;

            await Shell.Current.GoToAsync($"{nameof(DeckFormPage)}?deckId={deck.Id}");
        });

        DeleteDeckCommand = new Command<Deck>(async deck =>
        {
            if (deck is null)
                return;

            var shouldDelete = await Shell.Current.DisplayAlert(
                "Delete deck",
                $"Delete '{deck.Title}'?",
                "Delete",
                "Cancel");

            if (!shouldDelete)
                return;

            _deckStore.Delete(deck.Id);
        });
    }
}

