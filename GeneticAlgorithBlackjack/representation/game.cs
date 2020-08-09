using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeneticAlgorithBlackjack.Utils;
using GeneticAlgorithBlackjack.Engine;

namespace GeneticAlgorithBlackjack.Representation
{
    public enum GameState
    {
        PlayerBlackjack,
        PlayerDrawing,
        PlayerBusted,
        DealerDrawing,
        DealerBusted,
        HandComparison,
        RestartPlayerHand
    }

    class Game
    {
        private StrategyBase strategy;
        private TestConditions testConditions;

        public int GenerateFitness(int handsToPlay)
        {
            int playerChips = 0;
            var deck = new Deck(testConditions.numDecks);
            var randomizer = new Randomizer();

            Hand dealerHand = new Hand();
            Hand playerHand = new Hand();
            List<Hand> playerHands = new List<Hand>();
            List<int> betAmountPerHand = new List<int>();

            
        }

        

    }

    
}