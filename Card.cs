namespace Poker
{
    internal class Card
    {
        public CardRank Rank { get; set; }
        public CardSuit Suit { get; set; }
        public Card(CardRank rank, CardSuit suit)
        {
            Rank = rank;
            Suit = suit;
        }
        public string GetCard()
        {
            if ((int)Rank > 9) return $"{Rank.ToString()[0]}{Suit.ToString().ToLower()[0]}";
            else return $"{(int)Rank}{Suit.ToString().ToLower()[0]}";
        }
    }
}
