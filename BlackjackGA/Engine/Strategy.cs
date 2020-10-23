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

        public void Mutate(double impact)
        {
            // impact es el porcentaje de mutación que sufren los candidatos

            // calculamos la cantidad de celdas a mutar en cada matriz con respecto al impact
            int NumPairMutations = (int)(100F * impact);     // 10 posibles pares x 10 posibles cartas del dealer
            int NumSoftMutations = (int)(80F * impact);     // 8 posibles soft hands x 10 posibles cartas del dealer
            int NumHardMutations = (int)(160F * impact);     // 16 posibles hard hands x 10 posibles cartas del dealer

            // pares
            for (int i = 0; i < NumPairMutations; i++)
            {
                var upcardRank = GetRandomRankIndex();
                var randomPairRank = GetRandomRankIndex();
                SetActionForPair(upcardRank, randomPairRank, GetRandomAction(true));
            }

            // soft hands
            for (int i = 0; i < NumSoftMutations; i++)
            {
                var upcardRank = GetRandomRankIndex();
                var randomRemainder = randomizer.IntInRange(LowestSoftHandRemainder, HighestSoftHandRemainder);
                SetActionForSoftHand(upcardRank, randomRemainder, GetRandomAction(false));
            }

            // hard hands
            for (int i = 0; i < NumHardMutations; i++)
            {
                var upcardRank = GetRandomRankIndex();
                var hardTotal = randomizer.IntInRange(LowestHardHandValue, HighestHardHandValue);
                SetActionForHardHand(upcardRank, hardTotal, GetRandomAction(false));
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

        public void CrossOverWith(Strategy otherParent, Strategy child)
        {
            // here we create one child, with genetic information from each parent 
            // in proportion to their relative fitness scores
            float myScore = this.Fitness;
            float theirScore = otherParent.Fitness;
            float percentageChanceOfMine = 0;

            // it depends on whether the numbers are positive or negative
            if (myScore >= 0 && theirScore >= 0)
            {
                float totalScore = (myScore + theirScore);
                // safety check (avoiding / 0)
                if (totalScore < 0.001) totalScore = 1;
                percentageChanceOfMine = (myScore / totalScore);
            }
            else if (myScore >= 0 && theirScore < 0)
            {
                // hard to compare a positive and a negative, so let's tip the hat to Mr. Pareto
                percentageChanceOfMine = 0.8F;
            }
            else if (myScore < 0 && theirScore >= 0)
            {
                // hard to compare a positive and a negative, so let's tip the hat to Mr. Pareto
                percentageChanceOfMine = 0.2F;
            }
            else
            {
                // both negative, so use abs value and 1-(x)
                myScore = Math.Abs(myScore);
                theirScore = Math.Abs(theirScore);
                percentageChanceOfMine = 1 - (myScore / (myScore + theirScore));
            }

            for (int upcardRank = 0; upcardRank <= Card.HighestRankIndex; upcardRank++)
            {
                // populate the pairs
                for (int pairRank = 0; pairRank <= Card.HighestRankIndex; pairRank++)
                {
                    bool useMyAction = randomizer.RandomDoubleFromZeroToOne() < percentageChanceOfMine;
                    child.SetActionForPair(upcardRank, pairRank,
                        useMyAction ?
                            this.GetActionForPair(upcardRank, pairRank) :
                            otherParent.GetActionForPair(upcardRank, pairRank));
                }

                // populate the soft hands
                for (int softRemainder = LowestSoftHandRemainder; softRemainder <= HighestSoftHandRemainder; softRemainder++)
                {
                    bool useMyAction = randomizer.RandomDoubleFromZeroToOne() < percentageChanceOfMine;
                    child.SetActionForSoftHand(upcardRank, softRemainder,
                        useMyAction ?
                        this.GetActionForSoftHand(upcardRank, softRemainder) :
                        otherParent.GetActionForSoftHand(upcardRank, softRemainder));
                }

                // populate the hard hands
                for (int hardValue = LowestHardHandValue; hardValue <= HighestHardHandValue; hardValue++)
                {
                    bool useMyAction = randomizer.RandomDoubleFromZeroToOne() < percentageChanceOfMine;
                    child.SetActionForHardHand(upcardRank, hardValue,
                        useMyAction ?
                        this.GetActionForHardHand(upcardRank, hardValue) :
                        otherParent.GetActionForHardHand(upcardRank, hardValue));
                }
            }
        }

    }
}
