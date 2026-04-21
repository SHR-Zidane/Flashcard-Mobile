using System.Collections.ObjectModel;
using System.Text.Json;
using Flashcard_Mobile.Models;

namespace Flashcard_Mobile.Services;

public sealed class DeckStore
{
    private static readonly Lazy<DeckStore> _lazy = new(() => new DeckStore());
    public static DeckStore Instance => _lazy.Value;
    private readonly string _decksFilePath = Path.Combine(FileSystem.AppDataDirectory, "decks.json");
    private readonly List<Deck> _allDecks = new();

    public ObservableCollection<Deck> Decks { get; } = new();

    private DeckStore()
    {
        Load();
        SeedIfEmpty();
        RefreshVisibleDecks();
    }

    public Deck? GetById(Guid id)
    {
        return _allDecks.FirstOrDefault(d => d.Id == id && !d.IsDeleted);
    }

    public Deck Create(string title, string listName, int wordsCount)
    {
        var deck = new Deck
        {
            Id = Guid.NewGuid(),
            Title = title.Trim(),
            ListName = string.IsNullOrWhiteSpace(listName) ? "General" : listName.Trim(),
            WordsCount = wordsCount,
            IsDeleted = false,
            Flashcards = new List<Flashcard>()
        };

        _allDecks.Add(deck);
        Decks.Add(deck);
        Save();
        return deck;
    }

    public bool Update(Guid id, string title, string listName, int wordsCount)
    {
        var deck = _allDecks.FirstOrDefault(d => d.Id == id && !d.IsDeleted);
        if (deck is null)
            return false;

        var updatedDeck = new Deck
        {
            Id = deck.Id,
            Title = title.Trim(),
            ListName = string.IsNullOrWhiteSpace(listName) ? "General" : listName.Trim(),
            WordsCount = wordsCount,
            IsDeleted = false,
            Flashcards = deck.Flashcards
        };

        var allDeckIndex = _allDecks.IndexOf(deck);
        if (allDeckIndex >= 0)
            _allDecks[allDeckIndex] = updatedDeck;

        var visibleDeckIndex = Decks.IndexOf(deck);
        if (visibleDeckIndex >= 0)
            Decks[visibleDeckIndex] = updatedDeck;

        Save();
        return true;
    }

    public bool Delete(Guid id)
    {
        var deck = _allDecks.FirstOrDefault(d => d.Id == id && !d.IsDeleted);
        if (deck is null)
            return false;

        deck.IsDeleted = true;
        Decks.Remove(deck);
        Save();
        return true;
    }

    public Flashcard? AddFlashcard(Guid deckId, string front, string back)
    {
        var deck = _allDecks.FirstOrDefault(d => d.Id == deckId && !d.IsDeleted);
        if (deck is null)
            return null;

        var flashcard = new Flashcard
        {
            Id = Guid.NewGuid(),
            Front = front.Trim(),
            Back = back.Trim()
        };

        deck.Flashcards.Add(flashcard);
        deck.WordsCount = deck.Flashcards.Count;
        Save();
        return flashcard;
    }

    public bool UpdateFlashcard(Guid deckId, Guid flashcardId, string front, string back)
    {
        var deck = _allDecks.FirstOrDefault(d => d.Id == deckId && !d.IsDeleted);
        if (deck is null)
            return false;

        var flashcard = deck.Flashcards.FirstOrDefault(f => f.Id == flashcardId);
        if (flashcard is null)
            return false;

        flashcard.Front = front.Trim();
        flashcard.Back = back.Trim();
        Save();
        return true;
    }

    public bool DeleteFlashcard(Guid deckId, Guid flashcardId)
    {
        var deck = _allDecks.FirstOrDefault(d => d.Id == deckId && !d.IsDeleted);
        if (deck is null)
            return false;

        var flashcard = deck.Flashcards.FirstOrDefault(f => f.Id == flashcardId);
        if (flashcard is null)
            return false;

        deck.Flashcards.Remove(flashcard);
        deck.WordsCount = deck.Flashcards.Count;
        Save();
        return true;
    }

    private void SeedIfEmpty()
    {
        if (_allDecks.Count > 0)
            return;

        _allDecks.Add(new Deck
        {
            Title = "German Vocabulary",
            ListName = "Languages",
            Flashcards = new List<Flashcard>
            {
                new() { Front = "Hallo", Back = "Hello" },
                new() { Front = "Danke", Back = "Thank you" },
                new() { Front = "Auf Wiedersehen", Back = "Goodbye" }
            },
            WordsCount = 3
        });

        _allDecks.Add(new Deck
        {
            Title = "English Vocabulary",
            ListName = "Languages",
            Flashcards = new List<Flashcard>
            {
                new() { Front = "Apple", Back = "Pomme" },
                new() { Front = "Book", Back = "Livre" },
                new() { Front = "House", Back = "Maison" }
            },
            WordsCount = 3
        });

        _allDecks.Add(new Deck
        {
            Title = "French Vocabulary",
            ListName = "Languages",
            Flashcards = new List<Flashcard>
            {
                new() { Front = "Bonjour", Back = "Hello" },
                new() { Front = "Chat", Back = "Cat" },
                new() { Front = "Voiture", Back = "Car" }
            },
            WordsCount = 3
        });

        _allDecks.Add(new Deck
        {
            Title = "Spanish Vocabulary",
            ListName = "Languages",
            Flashcards = new List<Flashcard>
            {
                new() { Front = "Hola", Back = "Hello" },
                new() { Front = "Gracias", Back = "Thank you" },
                new() { Front = "Casa", Back = "House" }
            },
            WordsCount = 3
        });
        Save();
    }

    private void RefreshVisibleDecks()
    {
        Decks.Clear();
        foreach (var deck in _allDecks.Where(d => !d.IsDeleted))
        {
            Decks.Add(deck);
        }
    }

    private void Load()
    {
        try
        {
            if (!File.Exists(_decksFilePath))
            {
                return;
            }

            var json = File.ReadAllText(_decksFilePath);
            if (string.IsNullOrWhiteSpace(json))
            {
                return;
            }

            var loadedDecks = JsonSerializer.Deserialize<List<Deck>>(json);
            if (loadedDecks is null)
            {
                return;
            }

            _allDecks.Clear();
            _allDecks.AddRange(loadedDecks);
            foreach (var deck in _allDecks)
            {
                deck.Flashcards ??= new List<Flashcard>();
            }
        }
        catch
        {
            // Keep app usable even if JSON file is invalid/corrupted.
            _allDecks.Clear();
        }
    }

    private void Save()
    {
        try
        {
            var json = JsonSerializer.Serialize(_allDecks, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(_decksFilePath, json);
        }
        catch
        {
            // Ignore write errors to avoid crashing UI actions.
        }
    }
}
