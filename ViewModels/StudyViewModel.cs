using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Flashcard_Mobile.Models;
using Flashcard_Mobile.Services;
using Microsoft.Maui.Devices.Sensors;

namespace Flashcard_Mobile.ViewModels;

public class StudyViewModel : BindableObject
{
    private readonly DeckStore _deckStore = DeckStore.Instance;
    private readonly ShakeDetectionService _shakeDetectionService = new();
    private Deck? _deck;
    private List<Flashcard> _studyCards = new();
    private Dictionary<Guid, int> _cardErrors = new();
    private Stopwatch _stopwatch = new();
    private int _currentIndex = 0;
    private int _initialCardCount = 0;
    private int _fullyKnownCount = 0;
    private bool _showAnswer = false;
    private int _correctCount = 0;
    private int _incorrectCount = 0;
    private bool _isStudyComplete = false;
    private Random _random = new();

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
    public string TimeSpentText => $"Temps passé : {_stopwatch.Elapsed:mm\\:ss}";
    public string HardestCardText
    {
        get
        {
            if (_cardErrors.Count == 0 || _cardErrors.Values.All(v => v == 0))
                return "Carte la plus difficile : Aucune";
            int maxErrors = _cardErrors.Values.Max();
            var hardestId = _cardErrors.First(kvp => kvp.Value == maxErrors).Key;
            var card = _deck?.Flashcards.FirstOrDefault(c => c.Id == hardestId);
            return $"Carte la plus difficile : {card?.Front ?? "Inconnue"}";
        }
    }
    public string FullyKnownText
    {
        get
        {
            return $"Cartes connues à 100% : {_fullyKnownCount}";
        }
    }
    public string MemorizationText
    {
        get
        {
            if (_initialCardCount == 0) return "% de mémorisation : 0%";
            double percent = (double)_fullyKnownCount / _initialCardCount * 100;
            return $"% de mémorisation : {percent:F1}%";
        }
    }

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
            var currentCard = _studyCards[_currentIndex];
            bool hadErrors = _cardErrors.ContainsKey(currentCard.Id) && _cardErrors[currentCard.Id] > 0;

            if (!hadErrors)
                _fullyKnownCount++;

            _correctCount++;
            _studyCards.RemoveAt(_currentIndex);

            if (_studyCards.Count == 0)
            {
                FinishStudy();
            }
            else
            {
                if (_currentIndex >= _studyCards.Count)
                    _currentIndex = 0;
                ShowAnswer = false;
                OnPropertyChanged(nameof(CurrentFront));
                OnPropertyChanged(nameof(CurrentBack));
            }

            OnPropertyChanged(nameof(ResultText));
        });
        IncorrectCommand = new Command(() =>
        {
            _incorrectCount++;
            var currentCard = _studyCards[_currentIndex];
            if (_cardErrors.ContainsKey(currentCard.Id))
                _cardErrors[currentCard.Id]++;
            else
                _cardErrors[currentCard.Id] = 1;

            _studyCards.RemoveAt(_currentIndex);
            _studyCards.Add(currentCard);

            if (_currentIndex >= _studyCards.Count)
                _currentIndex = 0;

            ShowAnswer = false;
            OnPropertyChanged(nameof(CurrentFront));
            OnPropertyChanged(nameof(CurrentBack));
            OnPropertyChanged(nameof(ResultText));
        });
        RestartCommand = new Command(() =>
        {
            StartStudy(_deck);
        });

        QuitCommand = new Command(async () =>
        {
            if (!IsStudyComplete)
            {
                FinishStudy();
                return;
            }
            await Shell.Current.GoToAsync("..");
        });

        // Subscribe to shake detection
        _shakeDetectionService.ShakeDetected += OnShakeDetected;
    }

    private void FinishStudy()
    {
        _stopwatch.Stop();
        _shakeDetectionService.StopMonitoring();
        IsStudyComplete = true;
        OnPropertyChanged(nameof(ResultText));
        OnPropertyChanged(nameof(TimeSpentText));
        OnPropertyChanged(nameof(HardestCardText));
        OnPropertyChanged(nameof(FullyKnownText));
        OnPropertyChanged(nameof(MemorizationText));
    }
    
    private void OnShakeDetected(object? sender, EventArgs e)
    {
        if (!ShowAnswer || IsStudyComplete)
            return;
            
        IncorrectCommand.Execute(null);
    }

    public void StartStudy(Deck? deck)
    {
        _deck = deck;
        if (deck == null || deck.Flashcards.Count == 0)
        {
            return;
        }

        _studyCards = deck.Flashcards.OrderBy(x => Guid.NewGuid()).ToList();
        _initialCardCount = _studyCards.Count;
        _cardErrors = new Dictionary<Guid, int>();
        _stopwatch.Reset();
        _stopwatch.Start();
        _currentIndex = 0;
        _correctCount = 0;
        _incorrectCount = 0;
        _fullyKnownCount = 0;
        _isStudyComplete = false;
        ShowAnswer = false;
        
        _shakeDetectionService.StartMonitoring();

        OnPropertyChanged(nameof(CurrentFront));
        OnPropertyChanged(nameof(CurrentBack));
        OnPropertyChanged(nameof(ShowFront));
        OnPropertyChanged(nameof(ShowButtons));
        OnPropertyChanged(nameof(ShowResult));
    }
    
    public void StopMonitoring()
    {
        _shakeDetectionService.StopMonitoring();
    }
}