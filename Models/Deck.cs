using System.ComponentModel;

namespace Flashcard_Mobile.Models;

public class Deck : INotifyPropertyChanged
{
    private int _wordsCount;

    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string ListName { get; set; } = "General";

    public int WordsCount
    {
        get => _wordsCount;
        set
        {
            if (_wordsCount == value)
                return;

            _wordsCount = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(WordsCountText));
        }
    }

    public bool IsDeleted { get; set; }
    public List<Flashcard> Flashcards { get; set; } = new();

    public string WordsCountText => $"{Flashcards.Count} {(Flashcards.Count == 1 ? "carte" : "cartes")}";

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
