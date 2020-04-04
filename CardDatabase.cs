using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CAHCore
{
    public class CardDatabase
    {
        private static Random random = new Random();

        private static readonly List<Card> EmbeddedCards;

        private List<Card> PromptCards = new List<Card>(),
            ResponseCards = new List<Card>(),
            UsedPromptCards = new List<Card>(),
            UsedResponseCards = new List<Card>();

        public int RemainingPromptCards => PromptCards.Count;
        public int RemainingResponseCards => ResponseCards.Count;


        static CardDatabase()
        {
            EmbeddedCards = new List<Card>();

            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream("CAHCore.CAHCards.tsv");
            using (var reader = new StreamReader(stream))
            {
                string[] header = reader.ReadLine().Split('\t'); // column headings

                while (!reader.EndOfStream)
                {
                    var columns = reader.ReadLine().Split('\t');

                    if (columns.Length < 4 || string.IsNullOrWhiteSpace(columns[1]))
                        continue;

                    try
                    {
                        string cardType = columns[0],
                            cardContent = columns[1],
                            special = columns[2],
                            delimitedDecks = columns[3];

                        var decks = Utility.DecksFromList(delimitedDecks.Split('|', StringSplitOptions.RemoveEmptyEntries));

                        EmbeddedCards.Add(new Card
                        {
                            CardType = cardType == "Prompt" ? CardType.PromptBlack : CardType.ResponseWhite,
                            CardText = cardContent,
                            Decks = decks
                        });

                    }
                    catch (Exception e)
                    {
                    }
                }
            }



        }

        public CardDatabase(Decks decksToInclude = Decks.US__v1_0)
        {
            this.PromptCards = new List<Card>();
            this.ResponseCards = new List<Card>();

            foreach (var card in EmbeddedCards)
            {
                if ((card.Decks & decksToInclude) > 0)
                {
                    if (card.CardType == CardType.PromptBlack)
                    {
                        this.PromptCards.Add(card);
                    }
                    else if (card.CardType == CardType.ResponseWhite)
                    {
                        this.ResponseCards.Add(card);
                    }
                }
            }
        }

        public Card GetCard(CardType type)
        {
            if (type == CardType.PromptBlack)
            {
                if (RemainingPromptCards == 0)
                {
                    // Move all the used cards to new pile.
                    PromptCards = UsedPromptCards;
                    UsedPromptCards = new List<Card>();
                }

                var index = random.Next(0, PromptCards.Count);
                var card = this.PromptCards[index];
                PromptCards.RemoveAt(index);
                this.UsedPromptCards.Add(card);
                return card;
            }
            else
            {
                if (RemainingResponseCards == 0)
                {
                    ResponseCards = UsedResponseCards;
                    UsedResponseCards = new List<Card>();
                }


                var index = random.Next(0, ResponseCards.Count);
                var card = this.ResponseCards[index];
                ResponseCards.RemoveAt(index);
                this.UsedResponseCards.Add(card);
                return card;
            }
        }
    }
}
