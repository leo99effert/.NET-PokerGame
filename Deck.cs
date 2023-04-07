namespace Poker
{
    internal class Deck
    {
        public List<Card> Cards { get; set; } = new List<Card>();
        public Deck()
        {
            Cards.Clear();
            foreach (CardSuit suit in Enum.GetValues(typeof(CardSuit)))
                foreach (CardRank rank in Enum.GetValues(typeof(CardRank)))
                    Cards.Add(new Card(rank, suit));
        }

        public void Shuffle()
        {
            // Use Fisher-Yates shuffle algorithm to randomize the order of the cards
            Random random = new Random();
            int n = Cards.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                Card card = Cards[k];
                Cards[k] = Cards[n];
                Cards[n] = card;
            }
        }

        public Card DealTopCard()
        {
            Card topCard = Cards[0];
            Cards.RemoveAt(0);
            return topCard;
        }
    }

}
