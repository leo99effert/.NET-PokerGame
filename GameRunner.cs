﻿namespace Poker
{
    internal class GameRunner
    {
        public Deck Deck { get; set; } = new Deck();
        public int Pot { get; set; }
        public Player? LastBettingPlayer { get; set; }
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
        public void CleanUp()
        {
            foreach (var player in Players)
            {
                player.Action = Action.Waiting;
                player.Hand.Clear();
                player.CurrentBet = 0;
                player.MadeHand = MadeHand.HighCard;
                if (player.Position == Position.Button) player.Position = Position.SmallBlind;
                else player.Position++;
                Players = Players.OrderBy(p => p.Position).ToList();
                var firstPlayer = Players.First();
                var secondPlayer = Players.Skip(1).First();
                Players.Remove(firstPlayer);
                Players.Remove(secondPlayer);
                Players.Add(firstPlayer);
                Players.Add(secondPlayer);
            }
        }
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
            Console.Write($"{player.Position} - {player.Name}");
            int spacesForReadability = 32 - player.Name.Length - player.Position.ToString().Length;
            for (int i = 0; i < spacesForReadability; i++) Console.Write(" ");
            Console.Write(player.Hand[0].GetCard() + " ");
            Console.Write(player.Hand[1].GetCard() + " || ");
            Console.Write(player.StartingHandValue + " - ");
            Console.Write($"{player.Action} ({player.CurrentBet}Kr)\n");
            if (player.Position == Position.Button || player.Position == Position.UnderTheGun3 || player.Position == Position.BigBlind)
                Console.WriteLine();
        }

        public void PrintResult()
        {
            var board = new List<Card> { Deck!.DealTopCard(), Deck.DealTopCard(), Deck.DealTopCard(), Deck.DealTopCard(), Deck.DealTopCard() };
            Console.WriteLine();
            foreach (var card in board)
                Console.Write($"{card.GetCard()} ");
            foreach (var player in Players)
            {
                player.Hand.AddRange(board);
                player.Hand = player.Hand.OrderByDescending(c => (int)c.Rank).ToList();
            }
            Console.WriteLine();
            foreach (Player player in Players)
            {
                Console.Write($"{player.Name}");
                int spacesForReadability = 20 - player.Name.Length;
                for (int i = 0; i < spacesForReadability; i++) Console.Write(" ");
                Console.Write($"{player.Money} Kr || ");
                for (int i = 0; i < player.Hand.Count; i++)
                {
                    if (i > 0 && player.Hand[i].Rank == player.Hand[i - 1].Rank) Console.Write($"--{player.Hand[i].GetCard()}");
                    else Console.Write($"  {player.Hand[i].GetCard()}");
                }
                Console.WriteLine();
            }
        }

        public void PlayRound()
        {
            LastBettingPlayer = Players.FirstOrDefault(p => p.Position == Position.BigBlind)!;
            Pot = 0;
            while (Players.Any(p => p.Action == Action.Waiting)) // While players are still waiting to take action...
            {
                foreach (var player in Players) // For every player
                {
                    if (player.Action == Action.Waiting) // If their action is not yet decieded
                    {
                        if (player.StartingHandValue < 70) player.Action = Action.Fold; // Bad hands folds...
                        else
                        {
                            // List that can be used to check how many previos aggressor there has been
                            List<Action> aggressorNumberController = new List<Action>();
                            // If they are the first aggressor
                            aggressorNumberController.Add(Action.Waiting);
                            aggressorNumberController.Add(Action.Fold);
                            if (Players.All(p => aggressorNumberController.Contains(p.Action)))
                            {
                                // Then Bet! 
                                player.Action = Action.Bet;
                                player.CurrentBet = 6;
                                LastBettingPlayer = player;
                                foreach (var resetPlayer in Players)
                                    if (resetPlayer.Action != Action.Fold && resetPlayer != player) resetPlayer.Action = Action.Waiting;
                            }
                            // If they are the second aggressor
                            aggressorNumberController.Add(Action.Bet);
                            if (player.StartingHandValue >= 90 && player.Action == Action.Waiting &&
                                Players.All(p => aggressorNumberController.Contains(p.Action)))
                            {
                                // Then Raise!
                                player.Action = Action.Raise;
                                player.CurrentBet = 20;
                                LastBettingPlayer = player;

                                foreach (var resetPlayer in Players)
                                    if (resetPlayer.Action != Action.Fold && resetPlayer != player) resetPlayer.Action = Action.Waiting;
                            }
                            // If they are the third aggressor
                            aggressorNumberController.Add(Action.Raise);
                            if (player.StartingHandValue >= 95 && player.Action == Action.Waiting &&
                                Players.All(p => aggressorNumberController.Contains(p.Action)))
                            {
                                // Then ReRaise!
                                player.Action = Action.ReRaise;
                                player.CurrentBet = 50;
                                LastBettingPlayer = player;

                                foreach (var resetPlayer in Players)
                                    if (resetPlayer.Action != Action.Fold && resetPlayer != player) resetPlayer.Action = Action.Waiting;
                            }
                            // If they are the fourth aggressor or later
                            if (player.StartingHandValue == 100 && player.Action == Action.Waiting &&
                                Players.Any(p => p.Action == Action.ReRaise || p.Action == Action.AllIn))
                            {
                                // Then Go All In!
                                player.Action = Action.AllIn;
                                player.CurrentBet = player.Money;
                                LastBettingPlayer = player;

                                foreach (var resetPlayer in Players)
                                    if (resetPlayer.Action != Action.Fold &&
                                        resetPlayer.Action != Action.AllIn &&
                                        resetPlayer != player)
                                        resetPlayer.Action = Action.Waiting;
                            }
                            if (player.Action == Action.Waiting) player.Action = Action.Fold; // Fold good hand due to strong competition
                        }
                        PrintPlayer(player);
                    }
                }
            }
            foreach (Player player in Players)
            {
                if (player.Position == Position.SmallBlind && player.CurrentBet < 1) player.CurrentBet = 1;
                if (player.Position == Position.BigBlind && player.CurrentBet < 2) player.CurrentBet = 2;
                player.Money -= player.CurrentBet;
                Pot += player.CurrentBet;
            }
            LastBettingPlayer.Money += Pot;
        }
    }
}
