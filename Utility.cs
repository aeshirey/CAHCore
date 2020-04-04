using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static CAHCore.Settings;


namespace CAHCore
{
    internal static class Utility
    {
        public static void DrawHand(Canvas canvas, List<Card> cards, int? selection = null)
        {
            canvas.Children.Clear();

            DrawingVisual drawingVisual = new DrawingVisual();
            using (var draw = drawingVisual.RenderOpen())
            {
                for (int position = 0; position < cards.Count; position++)
                {
                    var card = cards[position];

                    var (backgroundBrush, textBrush) = card.CardType == CardType.PromptBlack
                        ? (blackBrush, whiteBrush)
                        : (whiteBrush, blackBrush);

                    var cardX = position * (CARD_SPACING + CARD_WIDTH);

                    draw.DrawRectangle(backgroundBrush, new Pen(textBrush, 1), new Rect(cardX, 0, CARD_WIDTH, CARD_HEIGHT));

                    var formattedText = new FormattedText(card.CardText,
                        CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight,
                        HelveticaBold,
                        24,
                        textBrush,
                        new NumberSubstitution(),
                        TextFormattingMode.Display, VisualTreeHelper.GetDpi(canvas).PixelsPerDip)
                    {
                        MaxTextWidth = CARD_WIDTH - 2 * CARD_TEXT_PADDING,
                        MaxTextHeight = CARD_HEIGHT - 2 * CARD_TEXT_PADDING,
                        Trimming = TextTrimming.None
                    };

                    var cardTextOrigin = new Point(cardX + CARD_TEXT_PADDING, CARD_TEXT_PADDING);
                    draw.DrawText(formattedText, cardTextOrigin);

                    if (selection.HasValue && selection.Value != position)
                    {
                        var opacityBrush = new SolidColorBrush(card.CardType == CardType.PromptBlack ? black : white) { Opacity = 0.75 };
                        draw.DrawRectangle(opacityBrush, new Pen(textBrush, 1), new Rect(cardX, 0, CARD_WIDTH, CARD_HEIGHT));
                    }
                }
            }

            RenderTargetBitmap rtb = new RenderTargetBitmap(
                (int)canvas.ActualWidth,
                (int)canvas.ActualHeight,
                96,
                96,
                PixelFormats.Default);

            rtb.Render(drawingVisual);
            Image image = new Image()
            {
                Source = rtb
            };
            canvas.Children.Add(image);
        }


        private static readonly string[] DecksMapping = new string[]
        {
            "US v1.0",
            "US v1.1",
            "US v1.2",
            "US v1.3",
            "US v1.4",
            "US v1.5",
            "UK v1.5",
            "US v1.6",
            "UK v1.6",
            "CA v1.6",
            "AU v1.6",
            "US v1.7",
            "CA v1.7",
            "UK v1.7",
            "AU v1.7",
            "CA v1.7a",
            "US v2.0",
            "CA v2.0",
            "UK v2.0",
            "AU v2.0",
            "INTL v2.0",
            "US v2.1",
        };

        /// <summary>
        /// Take a list of strings representing the decks we have and turn it into the [Flags] enum.
        /// </summary>
        /// <param name="decks">Strings rfom the embedded CAHCards.tsv (eg, "US v1.0")</param>
        /// <returns>A Decks enum representing the specified decks (eg, Decks.US__v1_0)</returns>
        /// <remarks>Interestingly, Enum.Parse is stupid slow. I had wanted to do a more functional style:
        ///     var decks = delimitedDecks
        ///        .Split('|', StringSplitOptions.RemoveEmptyEntries)
        ///        .Select(d => (Decks)Enum.Parse(typeof(Decks), d.Replace('.', '_').Replace(" ", "__")))
        ///        .Aggregate((d1, d2) => d1 | d2);
        /// But that takes 20 seconds (!) to run compared with this one which is sub-second. Meh.
        /// Eventually, I may remove the hard-coded values in favor of using Enum.GetNames; for an example,
        /// see https://stackoverflow.com/a/34775560/1191181
        ///</remarks>
        public static Decks DecksFromList(string[] decks)
        {
            Decks result = (Decks)0;
            foreach (string deck in decks)
            {
                int index = Array.IndexOf(Utility.DecksMapping, deck);
                if (index != -1)
                {
                    result |= (Decks)(1 << index);
                }
            }

            return result;
        }
    }
}
