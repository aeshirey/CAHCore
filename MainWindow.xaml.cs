using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using static CAHCore.Settings;

namespace CAHCore
{
    /// <summary>Interaction logic for MainWindow.xaml</summary>
    public partial class MainWindow : Window
    {

        private List<Card> HandOfCards;
        private CardDatabase cardDatabase;
        private int? selectedCard = null;

        private DispatcherTimer timer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(3) };

        private static DateTime expiry = new DateTime(2020, 4, 20, 0, 0, 0, 0);

        private int HandSize
        {
            get
            {
                if (HandSize5.IsChecked) return 5;
                if (HandSize6.IsChecked) return 6;
                if (HandSize7.IsChecked) return 7;
                if (HandSize8.IsChecked) return 8;
                return 5;
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            // When the timer ticks, clear the content
            this.timer.Tick += (timerSender, timerEvents) =>
            {
                sbLabel.Content = "";
                this.timer.Stop();
            };


            if (DateTime.Now > expiry)
            {
                MessageBox.Show("This version is expired.\n\nAs I am actively developing this app to make it suck less, I don't want crappy old versions hanging around. Contact adam.shirey@gmail.com for a new version.", "Expired", MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.OK);
                System.Windows.Application.Current.Shutdown();
            }
            else
            {
                this.HandOfCards = new List<Card>();
                this.cardDatabase = new CardDatabase();

                sbLabel.Content = $"Loaded {this.cardDatabase.RemainingPromptCards} prompt cards and {this.cardDatabase.RemainingResponseCards} response cards";
                this.timer.Start();
            }

            // Setup menu of different versions
            {
                for (int i = 0; i < Card.NUM_DECKS_AVAILABLE; i++)
                {
                    Decks deck = (Decks)(1UL << i);

                    var name = deck.ToString().Replace("__", " ").Replace("_", ".");
                    var mi = new MenuItem()
                    {
                        Header = name,
                        IsCheckable = true,
                        StaysOpenOnClick = true,
                        Tag = deck,
                        IsChecked = (i == 0) // only use US v1.0 by default
                    };

                    mi.Click += (miSender, miEventArgs) =>
                    {
                        sbLabel.Content = "Deck setting updated. Start a new game from the File menu to effect the change.";
                        timer.Start();
                    };

                    FileDecks.Items.Add(mi);
                }
            }
        }


        private void canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Point mousePos = e.GetPosition(canvas);
            if (mousePos.Y <= CARD_HEIGHT)
            {
                // was a card clicked? ignore space between them
                for (int position = 0; position < this.HandOfCards.Count; position++)
                {
                    var cardX = position * (CARD_SPACING + CARD_WIDTH);

                    if (cardX <= mousePos.X && mousePos.X <= cardX + CARD_WIDTH)
                    {
                        var card = this.HandOfCards[position];

                        if (selectedCard.HasValue && selectedCard.Value == position)
                        {
                            // we already had it selected. copy the text!
                            Clipboard.SetText(card.CardText);

                            // do a little notification
                            sbLabel.Content = "Copied card text to clipboard";
                            this.timer.Start();

                            // get a new card and stick it in the same position
                            this.HandOfCards[position] = cardDatabase.GetCard(CardType.ResponseWhite);
                            selectedCard = null;
                        }
                        else
                        {
                            // select this card
                            selectedCard = position;
                        }

                        Utility.DrawHand(canvas, this.HandOfCards, selectedCard);
                        break;
                    }
                }
            }
        }

        private void DealPromptCard_Click(object sender, RoutedEventArgs e)
        {
            var promptWindow = new PromptCard(this.cardDatabase);
            promptWindow.Show();

            ButtonAutomationPeer peer = new ButtonAutomationPeer(promptWindow.btnNewCard);
            IInvokeProvider provider = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;

            provider.Invoke();
        }


        private void SetHandSize_Click(object sender, RoutedEventArgs e)
        {
            // make sure the checkboxes are set correctly
            HandSize5.IsChecked = (sender == HandSize5);
            HandSize6.IsChecked = (sender == HandSize6);
            HandSize7.IsChecked = (sender == HandSize7);
            HandSize8.IsChecked = (sender == HandSize8);

            if (HandSize == this.HandOfCards.Count)
            {
                // nothing to do - bail!
                return;
            }

            if (HandSize < this.HandOfCards.Count)
            {
                // remove some cards
                this.HandOfCards.RemoveRange(HandSize, this.HandOfCards.Count - HandSize);
            }
            else
            {
                while (HandSize > this.HandOfCards.Count)
                {
                    // add a cards in
                    this.HandOfCards.Add(cardDatabase.GetCard(CardType.ResponseWhite));
                }
            }

            this.selectedCard = null;
            Utility.DrawHand(canvas, this.HandOfCards);
        }


        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // window resized -- redraw the canvas
            Utility.DrawHand(canvas, this.HandOfCards, this.selectedCard);
        }


        private void StartNewGame_Click(object sender, RoutedEventArgs e)
        {
            Decks decksToUse = (Decks)0;

            for (int i = 0; i < Card.NUM_DECKS_AVAILABLE; i++)
            {
                var mi = FileDecks.Items[i] as MenuItem;
                if (mi.IsChecked)
                {
                    var deck = (Decks)mi.Tag;
                    decksToUse |= deck;
                }
            }

            this.selectedCard = null;
            this.cardDatabase = new CardDatabase(decksToUse);
            this.HandOfCards = Enumerable
                .Range(0, HandSize)
                .Select(i => cardDatabase.GetCard(CardType.ResponseWhite))
                .ToList();

            Utility.DrawHand(canvas, this.HandOfCards);
        }
    }
}
