using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Flashcard_Mobile.Models;
using Flashcard_Mobile.Services;
using Flashcard_Mobile.Views;

namespace Flashcard_Mobile.ViewModels;

public class DeckDetailsViewModel : BindableObject
{
    private readonly DeckStore _deckStore = DeckStore.Instance;
    private Deck? _deck;

    public ObservableCollection<Flashcard> Flashcards { get; } = new();

    public string FlashcardsCountText => $"{Flashcards.Count} flashcards";

    public ICommand AddFlashcardCommand { get; }
    public ICommand EditFlashcardCommand { get; }
    public ICommand DeleteFlashcardCommand { get; }

    public DeckDetailsViewModel()
    {
        AddFlashcardCommand = new Command(async () =>
        {
            if (_deck is null)
                return;

            await Shell.Current.GoToAsync($"{nameof(FlashcardFormPage)}?deckId={_deck.Id}");
        });

        EditFlashcardCommand = new Command<Flashcard>(async flashcard =>
        {
            if (_deck is null || flashcard is null)
                return;

            await Shell.Current.GoToAsync($"{nameof(FlashcardFormPage)}?deckId={_deck.Id}&flashcardId={flashcard.Id}");
        });

        DeleteFlashcardCommand = new Command<Flashcard>(async flashcard =>
        {
            if (_deck is null || flashcard is null)
                return;

            var shouldDelete = await Shell.Current.DisplayAlert(
                "Delete flashcard",
                $"Delete this flashcard?",
                "Delete",
                "Cancel");

            if (!shouldDelete)
                return;

            _deckStore.DeleteFlashcard(_deck.Id, flashcard.Id);
            Flashcards.Remove(flashcard);
            OnPropertyChanged(nameof(FlashcardsCountText));
        });
    }

    public void SetDeck(Deck deck)
    {
        _deck = deck;
        Flashcards.Clear();
        foreach (var flashcard in deck.Flashcards)
        {
            Flashcards.Add(flashcard);
        }
        OnPropertyChanged(nameof(FlashcardsCountText));
    }
}