using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Flashcard_Mobile.Models;
using Flashcard_Mobile.Services;

namespace Flashcard_Mobile.ViewModels;

public class StudyViewModel : BindableObject
{
    private readonly DeckStore _deckStore = DeckStore.Instance;
    private Deck? _deck;
    private List<Flashcard> _studyCards = new();
    private int _currentIndex = 0;
    private bool _showAnswer = false;
    private int _correctCount = 0;
    private int _incorrectCount = 0;
    private bool _isStudyComplete = false;

    public ObservableCollection<Flashcard> StudyCards { get; } = new();
    public string CurrentFront => _currentIndex < _studyCards.Count ? _studyCards[_currentIndex].Front : string.Empty;
    public string CurrentBack => _currentIndex < _studyCards.Count ? _studyCards[_currentIndex].Back : string.Empty;
    public bool ShowAnswer
    {
        get => _showAnswer;
        set
        {
            _showAnswer = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ShowFront));
            OnPropertyChanged(nameof(ShowButtons));
        }
    }
    public bool ShowFront => !_showAnswer;
    public bool ShowButtons => _showAnswer && !_isStudyComplete;
    public bool IsStudyComplete
    {
        get => _isStudyComplete;
        set
        {
            _isStudyComplete = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ShowButtons));
            OnPropertyChanged(nameof(ShowResult));
        }
    }
    public bool ShowResult => _isStudyComplete;
    public string ResultText => $"Résultat: {_correctCount} correctes, {_incorrectCount} incorrectes";

    public ICommand ShowAnswerCommand { get; }
    public ICommand CorrectCommand { get; }
    public ICommand IncorrectCommand { get; }
    public ICommand RestartCommand { get; }
    public ICommand QuitCommand { get; }

    public StudyViewModel()
    {
        ShowAnswerCommand = new Command(() => ShowAnswer = true);
        CorrectCommand = new Command(() =>
        {
            _correctCount++;
            OnPropertyChanged(nameof(ResultText));
            NextCard();
        });
        IncorrectCommand = new Command(() =>
        {
            _incorrectCount++;
            OnPropertyChanged(nameof(ResultText));
            NextCard();
        });
        RestartCommand = new Command(() =>
        {
            StartStudy(_deck);
        });

        QuitCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
    }

    private void NextCard()
    {
        _currentIndex++;
        if (_currentIndex >= _studyCards.Count)
        {
            IsStudyComplete = true;
            OnPropertyChanged(nameof(ResultText));
        }
        else
        {
            ShowAnswer = false;
        }
        OnPropertyChanged(nameof(CurrentFront));
        OnPropertyChanged(nameof(CurrentBack));
    }

    public void StartStudy(Deck? deck)
    {
        _deck = deck;
        if (deck == null || deck.Flashcards.Count == 0)
        {
            return;
        }

        _studyCards = deck.Flashcards.OrderBy(x => Guid.NewGuid()).ToList();
        _currentIndex = 0;
        _correctCount = 0;
        _incorrectCount = 0;
        _isStudyComplete = false;
        ShowAnswer = false;

        OnPropertyChanged(nameof(CurrentFront));
        OnPropertyChanged(nameof(CurrentBack));
        OnPropertyChanged(nameof(ShowFront));
        OnPropertyChanged(nameof(ShowButtons));
        OnPropertyChanged(nameof(ShowResult));
    }
}