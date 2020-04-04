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

    public enum Expansion
    {
        BaseDeck
    }

    public class Card
    {
        public CardType CardType { get; private set; }
        public string CardText { get; private set; }
        public Card(CardType cardType, string cardText, Expansion expansion = Expansion.BaseDeck)
        {
            this.CardType = cardType;
            this.CardText = cardText;
        }
    }
}
