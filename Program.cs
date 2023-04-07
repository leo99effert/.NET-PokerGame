using Poker;


while (true)
{
    GameRunner gameRunner = new GameRunner();
    gameRunner.Deck = new Deck();
    gameRunner.Deck.Shuffle();
    gameRunner.DealStartingHands();
    gameRunner.SetActions();
    gameRunner.PrintResult();
    Console.ReadKey();
    Console.Clear();
}


