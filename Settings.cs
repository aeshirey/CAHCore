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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CAHCore
{
    public static class Settings
    {

        public const int CARD_SPACING = 15; // pixels between cards
        public const int CARD_TEXT_PADDING = 5; // pixels between the card edge and the text
        public const int CARD_WIDTH = 200;
        public const int CARD_HEIGHT = 300;
        
        public static readonly Color black = Color.FromRgb(0, 0, 0);
        public static readonly Color white = Color.FromRgb(255, 255, 255);
        public static readonly Brush blackBrush = new SolidColorBrush(black);
        public static readonly Brush whiteBrush = new SolidColorBrush(white);
        public static readonly Typeface HelveticaBold = new Typeface("Helvetica Bold");
    }
}
