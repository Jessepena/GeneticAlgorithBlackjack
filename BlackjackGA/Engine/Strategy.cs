using System;
using BlackjackGA.Representation;
using BlackjackGA.Utils;

namespace BlackjackGA.Engine
{
    // Esta clase encapsula una estrategia completa para jugar Blackjack
    class Strategy : StrategyBase
    {
        private Randomizer randomizer = new Randomizer();

        public Strategy Clone()
        {
            var result = new Strategy();
            result.DeepCopy(this);

            return result;
        }

        public void Randomize()
        {
            for (int upcardRank = 0; upcardRank <= Card.HighestRankIndex; upcardRank++)
            {
                // randomizar pares
                for (int pairRank = 0; pairRank <= Card.HighestRankIndex; pairRank++)
                    SetActionForPair(upcardRank, pairRank, GetRandomAction(true));

                // randomizar soft hands
                for (int softRemainder = 2; softRemainder <= HighestSoftHandRemainder; softRemainder++)
                    SetActionForSoftHand(upcardRank, softRemainder, GetRandomAction(false));

                // randomizar hard hands
                for (int hardValue = 5; hardValue <= HighestHardHandValue; hardValue++)
                    SetActionForHardHand(upcardRank, hardValue, GetRandomAction(false));
            }
        }

        private int GetRandomRankIndex()
        {
            return randomizer.Lesser(Card.HighestRankIndex);
        }

        private ActionToTake GetRandomAction(bool includeSplit)
        {
            return includeSplit ?
                (ActionToTake)randomizer.Lesser(NumActionsWithSplit) :
                (ActionToTake)randomizer.Lesser(NumActionsNoSplit);
        }


    }
}
