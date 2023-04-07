namespace Poker
{
    internal class Player
    {
        public string Name { get; set; } = string.Empty;
        public int Money { get; set; } = 10000;
        public int CurrentBet { get; set; }
        public List<Card> Hand { get; set; } = new List<Card>();
        public MadeHand MadeHand { get; set; } = MadeHand.HighCard;
        public Action Action { get; set; } = Action.Waiting;
        public int StartingHandValue { get; set; }
        public Position Position { get; set; }
        public Player(string name, Position position)
        {
            Name = name;
            Position = position;
        }
        public void SetStartingHandValue()
        {
            // First add the rank values
            StartingHandValue += (int)Hand[0].Rank;
            StartingHandValue += (int)Hand[1].Rank;
            // Add 10 for a pair
            if (Hand[0].Rank == Hand[1].Rank) StartingHandValue += 10;
            // Add 3 for suited
            if (Hand[0].Suit == Hand[1].Suit) StartingHandValue += 3;
            // Add 2 for connected
            int handGap = (int)Hand[0].Rank - (int)Hand[1].Rank;
            if (handGap == 1 || handGap == -1) StartingHandValue += 2;
            // Add 3 for an Ace
            if (Hand[0].Rank == CardRank.Ace) StartingHandValue += 3;
            if (Hand[1].Rank == CardRank.Ace) StartingHandValue += 3;
            // Add points for position
            StartingHandValue += (int)Position * 2;
        }
    }
}
