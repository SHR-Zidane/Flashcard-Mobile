namespace Flashcard_Mobile.Models;

public class Deck
{
    public string Title { get; set; } = string.Empty;
    public int WordsCount { get; set; }

    // Used directly by the UI (e.g., "200 words").
    public string WordsCountText => $"{WordsCount} words";
}
