namespace Poker
{
    internal class GameRunner
    {
        public Deck? Deck { get; set; }
        public List<Player> Players { get; set; } = new List<Player>
        {
            new Player("Doyle Brunson", Position.UnderTheGun),
            new Player("Phil Helmuth", Position.UnderTheGun2),
            new Player("Daniel Negreanu", Position.UnderTheGun3),
            new Player("Daniel Colman", Position.LoJack),
            new Player("Phil Ivey", Position.HiJack),
            new Player("Antonio Esfandiari", Position.CutOff),
            new Player("Erik Seidel", Position.Button),
            new Player("Johnny Chan", Position.SmallBlind),
            new Player("Tom Dwan", Position.BigBlind)
        };
        public void DealStartingHands()
        {
            foreach (Player player in Players)
            {
                player.Hand.Add(Deck!.DealTopCard());
                player.Hand.Add(Deck.DealTopCard());
                player.Hand = player.Hand.OrderByDescending(c => c.Rank).ToList();
                player.SetStartingHandValue();
                if (player.Hand[0].Rank == player.Hand[1].Rank) player.MadeHand = MadeHand.Pair;
            }
        }
        public void PrintPlayer(Player player)
        {
            Console.Write($"{player.Position} - {player.Name}\n");
            Console.Write(player.Hand[0].GetCard() + ", ");
            Console.Write(player.Hand[1].GetCard() + "\n");
            Console.Write(player.StartingHandValue + " - ");
            Console.Write(player.Action + "\n");
            if (player.Position == Position.Button || player.Position == Position.BigBlind || player.Position == Position.UnderTheGun3)
                Console.WriteLine();
        }

        public void PrintResult()
        {
            foreach (Player player in Players) Console.WriteLine($"{player.Name}: {player.Money}Kr");
        }

        public void SetActions()
        {
            while (Players.Any(p => p.Action == Poker.Action.Waiting)) // While players are still waiting to take action...
            {
                foreach (var player in Players) // For every player
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
                            if (Players.All(p => aggressorNumberController.Contains(p.Action)))
                            {
                                // Then Bet! 
                                player.Action = Poker.Action.Bet;
                                foreach (var resetPlayer in Players)
                                    if (resetPlayer.Action != Poker.Action.Fold && resetPlayer != player) resetPlayer.Action = Poker.Action.Waiting;
                            }
                            // If they are the second aggressor
                            aggressorNumberController.Add(Poker.Action.Bet);
                            if (player.StartingHandValue >= 80 && player.Action == Poker.Action.Waiting &&
                                Players.All(p => aggressorNumberController.Contains(p.Action)))
                            {
                                // Then Raise!
                                player.Action = Poker.Action.Raise;
                                foreach (var resetPlayer in Players)
                                    if (resetPlayer.Action != Poker.Action.Fold && resetPlayer != player) resetPlayer.Action = Poker.Action.Waiting;
                            }
                            // If they are the third aggressor
                            aggressorNumberController.Add(Poker.Action.Raise);
                            if (player.StartingHandValue >= 90 && player.Action == Poker.Action.Waiting &&
                                Players.All(p => aggressorNumberController.Contains(p.Action)))
                            {
                                // Then ReRaise!
                                player.Action = Poker.Action.ReRaise;
                                foreach (var resetPlayer in Players)
                                    if (resetPlayer.Action != Poker.Action.Fold && resetPlayer != player) resetPlayer.Action = Poker.Action.Waiting;
                            }
                            // If they are the fourth aggressor
                            aggressorNumberController.Add(Poker.Action.ReRaise);
                            if (player.StartingHandValue >= 95 && player.Action == Poker.Action.Waiting &&
                                Players.All(p => aggressorNumberController.Contains(p.Action)))
                            {
                                // Then Go All In!
                                player.Action = Poker.Action.AllIn;
                                foreach (var resetPlayer in Players)
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
    }
}
