using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace CAHCore
{
    public enum CardType
    {
        PromptBlack,
        ResponseWhite
    }


    [Flags]
    public enum Decks : UInt64
    {
        US__v1_0 = 1 << 0,
        US__v1_1 = 1 << 1,
        US__v1_2 = 1 << 2,
        US__v1_3 = 1 << 3,
        US__v1_4 = 1 << 4,
        US__v1_5 = 1 << 5,
        UK__v1_5 = 1 << 6,
        US__v1_6 = 1 << 7,
        UK__v1_6 = 1 << 8,
        CA__v1_6 = 1 << 9,
        AU__v1_6 = 1 << 10,
        US__v1_7 = 1 << 11,
        CA__v1_7 = 1 << 12,
        UK__v1_7 = 1 << 13,
        AU__v1_7 = 1 << 14,
        CA__v1_7a = 1 << 15,
        US__v2_0 = 1 << 16,
        CA__v2_0 = 1 << 17,
        UK__v2_0 = 1 << 18,
        AU__v2_0 = 1 << 19,
        INTL__v2_0 = 1 << 20,
        US__v2_1 = 1 << 21,
    }


    public class Card
    {
        internal const int NUM_DECKS_AVAILABLE = 21;
        public CardType CardType { get; set; }
        public string CardText { get; set; }
        public Decks Decks { get; set; }
    }
}
