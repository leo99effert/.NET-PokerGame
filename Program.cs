using Poker;

List<Player> players = new List<Player>
{
    new Player("player 3", Position.UnderTheGun),
    new Player("player 4", Position.UnderTheGun2),
    new Player("player 5", Position.UnderTheGun3),
    new Player("player 6", Position.LoJack),
    new Player("player 7", Position.HiJack),
    new Player("player 8", Position.CutOff),
    new Player("player 9", Position.Button),
    new Player("player 1", Position.SmallBlind),
    new Player("player 2", Position.BigBlind),
};

// Create a new deck of cards
Deck deck = new Deck();
// Shuffle the deck
deck.Shuffle();
// Create a list to store the dealt cards
List<Card> dealtCards = new List<Card>();
// Deal 2 cards to each player
foreach (Player player in players)
{
    for (int i = 0; i < 2; i++)
    {
        // Deal the top card from the deck
        Card card = deck.DealTopCard();

        // Add the card to the player's hand
        player.Hand.Add(card);

        // Add the card to the list of dealt cards
        dealtCards.Add(card);
    }
    player.SetStartingHandValue();
    player.Hand = player.Hand.OrderByDescending(c => c.Rank).ToList();
}

SetActions(players);

var playersWithPair = new List<Player>();
var playerWithHighCard = new List<Player>();
foreach (var player in players)
{
    if (player.Hand[0].Rank == player.Hand[1].Rank)
    {
        player.MadeHand = MadeHand.Pair;
        playersWithPair.Add(player);
    }
    else playerWithHighCard.Add(player);
}

static void PrintPlayer(Player player)
{
    Console.Write($"{player.Position}: ");
    Console.Write(player.Hand[0].GetCard() + ", ");
    Console.Write(player.Hand[1].GetCard() + ", ");
    Console.Write(player.StartingHandValue + ", ");
    Console.Write(player.Action + "\n\n");
    if (player.Position == Position.Button || player.Position == Position.BigBlind || player.Position == Position.UnderTheGun3)
        Console.WriteLine();
}

static void SetActions(List<Player> players)
{
    while (players.Any(p => p.Action == Poker.Action.Waiting)) // While players are still waiting to take action...
    {
        foreach (var player in players) // For every player
        {
            if (player.Action == Poker.Action.Waiting) // If their action is not yet decieded
            {
                if (player.StartingHandValue < 32) player.Action = Poker.Action.Fold; // Bad hands folds...
                else
                {
                    // If they are the first aggressor
                    if (players.All(p => p.Action == Poker.Action.Waiting || p.Action == Poker.Action.Fold))
                    {
                        // Then Bet! 
                        player.Action = Poker.Action.Bet;
                        foreach (var resetPlayer in players)
                            if (resetPlayer.Action != Poker.Action.Fold && resetPlayer != player) resetPlayer.Action = Poker.Action.Waiting;
                    }
                    // If they are the second aggressor
                    else if (player.StartingHandValue >= 40 &&
                        players.All(p => p.Action == Poker.Action.Waiting || p.Action == Poker.Action.Fold || p.Action == Poker.Action.Bet))
                    {
                        // Then Raise!
                        player.Action = Poker.Action.Raise;
                        foreach (var resetPlayer in players)
                            if (resetPlayer.Action != Poker.Action.Fold && resetPlayer != player) resetPlayer.Action = Poker.Action.Waiting;
                    }
                    // If they are the third aggressor
                    else if (player.StartingHandValue >= 45 &&
                        players.All(p => p.Action == Poker.Action.Waiting ||
                                               p.Action == Poker.Action.Fold ||
                                               p.Action == Poker.Action.Bet ||
                                               p.Action == Poker.Action.Raise))
                    {
                        // Then ReRaise!
                        player.Action = Poker.Action.ReRaise;
                        foreach (var resetPlayer in players)
                            if (resetPlayer.Action != Poker.Action.Fold && resetPlayer != player) resetPlayer.Action = Poker.Action.Waiting;
                    }
                    // If they are the fourth aggressor
                    else if (player.StartingHandValue >= 50)
                    {
                        // Then Go All In!
                        player.Action = Poker.Action.AllIn;
                        foreach (var resetPlayer in players)
                            if (resetPlayer.Action != Poker.Action.Fold &&
                                resetPlayer.Action != Poker.Action.AllIn &&
                                resetPlayer != player)
                                resetPlayer.Action = Poker.Action.Waiting;
                    }
                    else player.Action = Poker.Action.Fold; // Fold good hand due to strong competition
                }
                PrintPlayer(player);
                Console.WriteLine();
            }
        }
    }
}