using Poker;

List<Player> players = new List<Player>
{
    new Player("Doyle Brunson", Position.UnderTheGun),
    new Player("Phil Helmuth", Position.UnderTheGun2),
    new Player("Daniel Negreanu", Position.UnderTheGun3),
    new Player("Daniel Colman", Position.LoJack),
    new Player("Phil Ivey", Position.HiJack),
    new Player("Antonio Esfandiari", Position.CutOff),
    new Player("Erik Seidel", Position.Button),
    new Player("Johnny Chan", Position.SmallBlind),
    new Player("Tom Dwan", Position.BigBlind),
};

Deck deck = new Deck();
deck.Shuffle();
foreach (Player player in players)
{
    player.Hand.Add(deck.DealTopCard());
    player.Hand.Add(deck.DealTopCard());
    player.Hand = player.Hand.OrderByDescending(c => c.Rank).ToList();
    player.SetStartingHandValue();
}

SetActions(players);
foreach (var player in players) if (player.Hand[0].Rank == player.Hand[1].Rank) player.MadeHand = MadeHand.Pair;

static void PrintPlayer(Player player)
{
    Console.Write($"{player.Position} - {player.Name}\n");
    Console.Write(player.Hand[0].GetCard() + ", ");
    Console.Write(player.Hand[1].GetCard() + "\n");
    Console.Write(player.StartingHandValue + " - ");
    Console.Write(player.Action + "\n");
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
                if (player.StartingHandValue < 70) player.Action = Poker.Action.Fold; // Bad hands folds...
                else
                {
                    // List that can be used to check how many previos aggressor there has been
                    List<Poker.Action> aggressorNumberController = new List<Poker.Action>();
                    // If they are the first aggressor
                    aggressorNumberController.Add(Poker.Action.Waiting);
                    aggressorNumberController.Add(Poker.Action.Fold);
                    if (players.All(p => aggressorNumberController.Contains(p.Action)))
                    {
                        // Then Bet! 
                        player.Action = Poker.Action.Bet;
                        foreach (var resetPlayer in players)
                            if (resetPlayer.Action != Poker.Action.Fold && resetPlayer != player) resetPlayer.Action = Poker.Action.Waiting;
                    }
                    // If they are the second aggressor
                    aggressorNumberController.Add(Poker.Action.Bet);
                    if (player.StartingHandValue >= 80 && player.Action == Poker.Action.Waiting &&
                        players.All(p => aggressorNumberController.Contains(p.Action)))
                    {
                        // Then Raise!
                        player.Action = Poker.Action.Raise;
                        foreach (var resetPlayer in players)
                            if (resetPlayer.Action != Poker.Action.Fold && resetPlayer != player) resetPlayer.Action = Poker.Action.Waiting;
                    }
                    // If they are the third aggressor
                    aggressorNumberController.Add(Poker.Action.Raise);
                    if (player.StartingHandValue >= 90 && player.Action == Poker.Action.Waiting &&
                        players.All(p => aggressorNumberController.Contains(p.Action)))
                    {
                        // Then ReRaise!
                        player.Action = Poker.Action.ReRaise;
                        foreach (var resetPlayer in players)
                            if (resetPlayer.Action != Poker.Action.Fold && resetPlayer != player) resetPlayer.Action = Poker.Action.Waiting;
                    }
                    // If they are the fourth aggressor
                    aggressorNumberController.Add(Poker.Action.ReRaise);
                    if (player.StartingHandValue >= 95 && player.Action == Poker.Action.Waiting &&
                        players.All(p => aggressorNumberController.Contains(p.Action)))
                    {
                        // Then Go All In!
                        player.Action = Poker.Action.AllIn;
                        foreach (var resetPlayer in players)
                            if (resetPlayer.Action != Poker.Action.Fold &&
                                resetPlayer.Action != Poker.Action.AllIn &&
                                resetPlayer != player)
                                resetPlayer.Action = Poker.Action.Waiting;
                    }
                    if (player.Action == Poker.Action.Waiting) player.Action = Poker.Action.Fold; // Fold good hand due to strong competition
                }
                PrintPlayer(player);
                Console.WriteLine();
            }
        }
    }
}