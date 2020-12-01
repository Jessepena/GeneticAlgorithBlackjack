using System;
using System.Linq;
using BlackjackGA.Representation;

namespace BlackjackGA.Engine
{

    // enum de las acciones
    public enum ActionToTake { Stand, Hit, Double, Split };

    abstract class StrategyBase
    {
        public static int NumActionsNoSplit = 3;
        public static int NumActionsWithSplit = 4;

        public static int HighestUpcardIndex = 9;

        // estas constantes son los límites de las cartas que acompañarían al As en una soft hand
        // estas cartas van de 2 a 9 debido a que A con 10 sería blackjack y A con 1, sería un par y no una soft hand
        public static int LowestSoftHandRemainder = 2;
        public static int HighestSoftHandRemainder = 9;

        // estas constantes son los límites del valor de una hard hard.
        // estas empiezan en 5 debido a que es lo mas bajito que se puede obtener sin tener un par (2-2) y va hasta 20 
        // debido a que 21 sería blackjack y es automaticamente stand
        public static int LowestHardHandValue = 5;
        public static int HighestHardHandValue = 20;

        public float Fitness { get; set; } = 0;

        public ActionToTake[,] pairsStrategy, softStrategy, hardStrategy;

        public StrategyBase()
        {
            // Se inicializan las matrices con el valor por default stand
            pairsStrategy = new ActionToTake[HighestUpcardIndex + 1, HighestUpcardIndex + 1];
            softStrategy = new ActionToTake[HighestUpcardIndex + 1, HighestSoftHandRemainder + 1];
            hardStrategy = new ActionToTake[HighestUpcardIndex + 1, HighestHardHandValue + 1];
        }

        public void DeepCopy(StrategyBase copyFrom)
        {
            this.Fitness = copyFrom.Fitness;
            pairsStrategy = (ActionToTake[,])copyFrom.pairsStrategy.Clone();
            softStrategy = (ActionToTake[,])copyFrom.softStrategy.Clone();
            hardStrategy = (ActionToTake[,])copyFrom.hardStrategy.Clone();
        }

        // getters y setters para las tres tablas o matrices: hard, soft y pairs
        public ActionToTake GetActionForPair(int upcardRank, int pairRank)
        {
            return pairsStrategy[upcardRank, pairRank];
        }
        public void SetActionForPair(int upcardRank, int pairRank, ActionToTake action)
        {
            pairsStrategy[upcardRank, pairRank] = action;
        }

        public ActionToTake GetActionForSoftHand(int upcardRank, int softRemainder)
        {
            return softStrategy[upcardRank, softRemainder];
        }
        public void SetActionForSoftHand(int upcardRank, int softRemainder, ActionToTake action)
        {
            softStrategy[upcardRank, softRemainder] = action;
        }

        public ActionToTake GetActionForHardHand(int upcardRank, int hardTotal)
        {
            return hardStrategy[upcardRank, hardTotal];
        }
        public void SetActionForHardHand(int upcardRank, int hardTotal, ActionToTake action)
        {
            hardStrategy[upcardRank, hardTotal] = action;
        }


        // funcion que te devuelve la acción a tomar dado una mano y la carta del dealer
        public virtual ActionToTake GetActionForHand(Hand hand, Card dealerUpcard)
        {
            if (hand.HandValue() >= 21) return ActionToTake.Stand;

            var upcardIndex = IndexFromRank(dealerUpcard.Rank);

            if (hand.IsPair())
            {
                var pairIndex = IndexFromRank(hand.Cards[0].Rank);
                return pairsStrategy[upcardIndex, pairIndex];
            }

            if (hand.HasSoftAce())
            {
                // aquí usamos como índice la suma de todas las cartas, sin tomar en cuenta el As que está valiendo 11
                int howManyAces = hand.Cards.Count(c => c.Rank == Card.Ranks.Ace);
                int total = hand.Cards
                    .Where(c => c.Rank != Card.Ranks.Ace)
                    .Sum(c => c.RankValueHigh) +
                    (howManyAces - 1);

                return softStrategy[upcardIndex, total];
            }

            return hardStrategy[upcardIndex, hand.HandValue()];
        }

        public virtual ActionToTake GetActionForHand(Hand hand, Card dealerUpcard, int trueCount)
        {
            if (hand.HandValue() >= 21) return ActionToTake.Stand;

            var upcardIndex = IndexFromRank(dealerUpcard.Rank);

            if (hand.IsPair())
            {
                var pairIndex = IndexFromRank(hand.Cards[0].Rank);
                return pairsStrategy[upcardIndex, pairIndex];
            }

            if (hand.HasSoftAce())
            {
                // aquí usamos como índice la suma de todas las cartas, sin tomar en cuenta el As que está valiendo 11
                int howManyAces = hand.Cards.Count(c => c.Rank == Card.Ranks.Ace);
                int total = hand.Cards
                    .Where(c => c.Rank != Card.Ranks.Ace)
                    .Sum(c => c.RankValueHigh) +
                    (howManyAces - 1);

                return softStrategy[upcardIndex, total];
            }

            return hardStrategy[upcardIndex, hand.HandValue()];
        }

        public int IndexFromRank(Card.Ranks rank)
        {
            // aquí hay ciertas cartas juntas y esto se debe a que tienen el mismo valor y, por lo tanto, el mismo indice
            switch (rank)
            {
                case Card.Ranks.Ace:
                    return 9;

                case Card.Ranks.King:
                case Card.Ranks.Queen:
                case Card.Ranks.Jack:
                case Card.Ranks.Ten:
                    return 8;

                case Card.Ranks.Nine:
                    return 7;

                case Card.Ranks.Eight:
                    return 6;

                case Card.Ranks.Seven:
                    return 5;

                case Card.Ranks.Six:
                    return 4;

                case Card.Ranks.Five:
                    return 3;

                case Card.Ranks.Four:
                    return 2;

                case Card.Ranks.Three:
                    return 1;

                case Card.Ranks.Two:
                    return 0;
            }
            throw new InvalidOperationException();
        }
    }
}
