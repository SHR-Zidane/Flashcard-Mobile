using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Flashcard_Mobile.Models;

namespace Flashcard_Mobile.ViewModels;

public class MainPageViewModel
{
    public ObservableCollection<Deck> Decks { get; } = new();

    public ICommand ModifyDeckCommand { get; }
    public ICommand DeleteDeckCommand { get; }

    public MainPageViewModel()
    {
        // Données de démo pour remplir la home.
        Decks.Add(new Deck { Title = "German Vocabulary, Vocabulaire / apprendre", WordsCount = 200 });
        Decks.Add(new Deck { Title = "English Vocabulary, Vocabulaire / apprendre", WordsCount = 180 });
        Decks.Add(new Deck { Title = "French Vocabulary, Vocabulaire / apprendre", WordsCount = 160 });
        Decks.Add(new Deck { Title = "Spanish Vocabulary, Vocabulaire / apprendre", WordsCount = 140 });

        ModifyDeckCommand = new Command<Deck>(deck => { /* TODO: Navigation vers l'écran de modification */ });
        DeleteDeckCommand = new Command<Deck>(deck =>
        {
            if (deck is null)
                return;

            Decks.Remove(deck);
        });
    }
}

