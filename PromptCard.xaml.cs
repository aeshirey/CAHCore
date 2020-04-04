using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static CAHCore.Settings;

namespace CAHCore
{
    /// <summary>
    /// Interaction logic for PromptCard.xaml
    /// </summary>
    public partial class PromptCard : Window
    {
        private readonly CardDatabase cardDatabase;
        private Card card;
        public PromptCard(CardDatabase cardDatabase)
        {
            InitializeComponent();

            this.cardDatabase = cardDatabase;
            this.Width = Settings.CARD_WIDTH;
            this.Height = Settings.CARD_HEIGHT + 20;
        }

        private void btnCopyText_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnNewCard_Click(object sender, RoutedEventArgs e)
        {
            this.card = cardDatabase.GetCard(CardType.PromptBlack);
            Utility.DrawHand(this.canvas, new List<Card> { this.card });
        }


    }
}
