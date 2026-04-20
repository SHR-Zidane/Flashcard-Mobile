namespace Flashcard_Mobile.Models;

public class Deck
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string ListName { get; set; } = "General";
    public int WordsCount { get; set; }
    public bool IsDeleted { get; set; }
    public List<Flashcard> Flashcards { get; set; } = new();

    // Used directly by the UI (e.g., "200 words").
    public string WordsCountText => $"{WordsCount} words";
}
