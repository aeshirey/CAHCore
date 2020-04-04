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

        private List<Card> PromptCards = new List<Card>(),
            ResponseCards = new List<Card>(),
            UsedPromptCards = new List<Card>(),
            UsedResponseCards = new List<Card>();

        public int RemainingPromptCards => PromptCards.Count;
        public int RemainingResponseCards => ResponseCards.Count;


        public CardDatabase()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream("CAHCore.CAHCards.tsv");
            using (var reader = new StreamReader(stream))
            {
                string[] header = reader.ReadLine().Split('\t'); // column headings

                // for now, we'll skip anything that isn't a US vX.X card
                var usColumns = header
                    .Select((col, i) => Tuple.Create(col, i))
                    .Where(tup => tup.Item1.Contains("US v"))
                    .Select(tup => tup.Item2)
                    .ToArray();

                var maxCol = usColumns.Max();

                while (!reader.EndOfStream)
                {
                    var columns = reader.ReadLine().Split('\t');

                    if (columns.Length < 2 || string.IsNullOrWhiteSpace(columns[1]))
                        continue;

                    try
                    {
                        // keep only US cols
                        if (columns.Length > maxCol && usColumns.Any(index => !string.IsNullOrWhiteSpace(columns[index])))
                        {
                            var content = columns[1].Replace("\\n", "\n");
                            if (columns[0] == "Prompt")
                            {
                                this.PromptCards.Add(new Card(CardType.PromptBlack, content));
                            }
                            else if (columns[0] == "Response")
                            {
                                this.ResponseCards.Add(new Card(CardType.ResponseWhite, content));
                            }
                        }
                    }
                    catch
                    {
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
