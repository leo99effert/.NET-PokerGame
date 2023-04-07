using Poker;


GameRunner gameRunner = new GameRunner();
while (true)
{
    Console.Clear();
    gameRunner.Deck!.ResetDeck();
    gameRunner.Deck.Shuffle();
    gameRunner.DealStartingHands();
    gameRunner.SetActions();
    gameRunner.PrintResult();
    Console.ReadKey();
}


