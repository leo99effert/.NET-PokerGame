namespace Poker
{
    internal class Deck
    {
        private List<Card> cards;

        public Deck()
        {
            // Create a new list of cards
            cards = new List<Card>();

            // Loop through each suit and rank to create a full deck
            foreach (CardSuit suit in Enum.GetValues(typeof(CardSuit)))
            {
                foreach (CardRank rank in Enum.GetValues(typeof(CardRank)))
                {
                    // Create a new card with the current suit and rank
                    Card card = new Card(rank, suit);

                    // Add the card to the deck
                    cards.Add(card);
                }
            }
        }

        public void Shuffle()
        {
            // Use Fisher-Yates shuffle algorithm to randomize the order of the cards
            Random random = new Random();
            int n = cards.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                Card card = cards[k];
                cards[k] = cards[n];
                cards[n] = card;
            }
        }

        public Card DealTopCard()
        {
            // Get the top card from the deck
            Card topCard = cards[0];

            // Remove the top card from the deck
            cards.RemoveAt(0);

            // Return the top card
            return topCard;
        }
    }

}
