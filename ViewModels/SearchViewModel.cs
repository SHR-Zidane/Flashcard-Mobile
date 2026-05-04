using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Flashcard_Mobile.Models;
using Flashcard_Mobile.Services;
using Flashcard_Mobile.Views;

namespace Flashcard_Mobile.ViewModels;

public class SearchViewModel : BindableObject
{
    private readonly DeckStore _deckStore = DeckStore.Instance;
    private string _searchText = string.Empty;

    public ObservableCollection<Deck> SearchResults { get; } = new();
    public ObservableCollection<Deck> AllDecks => _deckStore.Decks;

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (_searchText != value)
            {
                _searchText = value;
                OnPropertyChanged();
                PerformSearch();
            }
        }
    }

    public ICommand OpenDeckCommand { get; }

    public SearchViewModel()
    {
        OpenDeckCommand = new Command<Deck>(async deck =>
        {
            if (deck is null)
                return;

            await Shell.Current.GoToAsync($"{nameof(DeckDetailsPage)}?deckId={deck.Id}");
        });

        // Initialize with all decks
        PerformSearch();
    }

    private void PerformSearch()
    {
        SearchResults.Clear();

        if (string.IsNullOrWhiteSpace(_searchText))
        {
            // Show all decks when search is empty
            foreach (var deck in AllDecks)
            {
                SearchResults.Add(deck);
            }
        }
        else
        {
            // Filter decks by title (case-insensitive)
            var filteredDecks = AllDecks.Where(d =>
                d.Title.Contains(_searchText, StringComparison.OrdinalIgnoreCase));

            foreach (var deck in filteredDecks)
            {
                SearchResults.Add(deck);
            }
        }
    }
}