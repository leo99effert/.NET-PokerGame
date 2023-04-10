using Poker;


GameRunner gameRunner = new GameRunner();
while (true)
{
    Console.Clear();
    gameRunner.Deck!.ResetDeck();
    gameRunner.Deck.Shuffle();
    gameRunner.DealStartingHands();
    gameRunner.PlayRound();
    gameRunner.PrintResult();
    gameRunner.CleanUp();
    Console.ReadKey();
}


