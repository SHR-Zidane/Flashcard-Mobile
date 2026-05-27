using System.ComponentModel;
using Flashcard_Mobile.Services;
using Flashcard_Mobile.ViewModels;

namespace Flashcard_Mobile.Views;

[QueryProperty(nameof(DeckId), "deckId")]
public partial class StudyPage : ContentPage
{
    private readonly DeckStore _deckStore = DeckStore.Instance;
    private readonly StudyViewModel _viewModel;
    private bool _isAnimating;
    private bool _hasStarted;

    public string DeckId
    {
        set
        {
            if (!Guid.TryParse(value, out var id))
            {
                return;
            }

            var deck = _deckStore.GetById(id);
            if (deck == null)
            {
                DisplayAlert("Erreur", "Deck introuvable", "OK");
                Shell.Current.GoToAsync("..");
                return;
            }

            if (deck.Flashcards.Count == 0)
            {
                DisplayAlert("Impossible", "Aucune flashcard dans ce deck. Ajoutez des flashcards avant d'étudier.", "OK");
                return;
            }

            _viewModel.StartStudy(deck);
        }
    }

    public StudyPage()
    {
        InitializeComponent();
        _viewModel = new StudyViewModel();
        BindingContext = _viewModel;
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        UpdateLabelVisibility(false);
    }

    private void UpdateLabelVisibility(bool showAnswer)
    {
        FrontLabel.IsVisible = !showAnswer;
        BackLabel.IsVisible = showAnswer;
        HintLabel.IsVisible = !showAnswer;
    }

    private async void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(StudyViewModel.ShowAnswer))
            return;

        if (!_hasStarted)
        {
            _hasStarted = true;
            UpdateLabelVisibility(_viewModel.ShowAnswer);
            return;
        }

        await AnimateCardFlip(_viewModel.ShowAnswer);
    }

    private async Task AnimateCardFlip(bool showAnswer)
    {
        if (_isAnimating)
            return;

        _isAnimating = true;

        try
        {
            await CardFrame.RotateYTo(90, 200, Easing.CubicIn);

            FrontLabel.IsVisible = !showAnswer;
            BackLabel.IsVisible = showAnswer;
            HintLabel.IsVisible = !showAnswer;

            CardFrame.RotationY = 270;
            await CardFrame.RotateYTo(360, 200, Easing.CubicOut);
            CardFrame.RotationY = 0;
        }
        finally
        {
            _isAnimating = false;
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.StopMonitoring();
    }
}