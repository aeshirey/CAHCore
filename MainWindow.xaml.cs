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
            // how many?
            int handSize;
            if (sender == HandSize5)
                handSize = 5;
            else if (sender == HandSize6)
                handSize = 6;
            else if (sender == HandSize7)
                handSize = 7;
            else if (sender == HandSize8)
                handSize = 8;
            else
                handSize = 5;

            if (handSize == this.HandOfCards.Count)
            {
                // nothing to do - bail!
                return;
            }

            if (handSize < this.HandOfCards.Count)
            {
                // remove some cards
                this.HandOfCards.RemoveRange(handSize, this.HandOfCards.Count - handSize);
            }
            else
            {
                while (handSize > this.HandOfCards.Count)
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
    }
}
