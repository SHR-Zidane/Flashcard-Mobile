using System.Collections.ObjectModel;
using Flashcard_Mobile.Models;

namespace Flashcard_Mobile.Services;

public sealed class DeckStore
{
    private static readonly Lazy<DeckStore> _lazy = new(() => new DeckStore());
    public static DeckStore Instance => _lazy.Value;

    public ObservableCollection<Deck> Decks { get; } = new();

    private DeckStore()
    {
        SeedIfEmpty();
    }

    public Deck? GetById(Guid id)
    {
        return Decks.FirstOrDefault(d => d.Id == id);
    }

    public Deck Create(string title, string listName, int wordsCount)
    {
        var deck = new Deck
        {
            Id = Guid.NewGuid(),
            Title = title.Trim(),
            ListName = string.IsNullOrWhiteSpace(listName) ? "General" : listName.Trim(),
            WordsCount = wordsCount
        };

        Decks.Add(deck);
        return deck;
    }

    public bool Update(Guid id, string title, string listName, int wordsCount)
    {
        var deck = GetById(id);
        if (deck is null)
            return false;

        deck.Title = title.Trim();
        deck.ListName = string.IsNullOrWhiteSpace(listName) ? "General" : listName.Trim();
        deck.WordsCount = wordsCount;
        return true;
    }

    public bool Delete(Guid id)
    {
        var deck = GetById(id);
        if (deck is null)
            return false;

        Decks.Remove(deck);
        return true;
    }

    private void SeedIfEmpty()
    {
        if (Decks.Count > 0)
            return;

        Decks.Add(new Deck { Title = "German Vocabulary", ListName = "Languages", WordsCount = 200 });
        Decks.Add(new Deck { Title = "English Vocabulary", ListName = "Languages", WordsCount = 180 });
        Decks.Add(new Deck { Title = "French Vocabulary", ListName = "Languages", WordsCount = 160 });
        Decks.Add(new Deck { Title = "Spanish Vocabulary", ListName = "Languages", WordsCount = 140 });
    }
}
