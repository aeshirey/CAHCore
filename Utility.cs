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

    }
}
