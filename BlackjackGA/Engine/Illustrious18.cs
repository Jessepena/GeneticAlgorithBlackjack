using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackjackGA.Representation;

namespace BlackjackGA.Engine
{
    class Illustrious18 : BasicStrategy
    {

        public override ActionToTake GetActionForHand(Hand hand, Card dealerUpcard, int trueCount)
        {
            if (hand.HandValue() >= 21) return ActionToTake.Stand;

            var upcardIndex = IndexFromRank(dealerUpcard.Rank);

            if (hand.IsPair())
            {
                var pairIndex = IndexFromRank(hand.Cards[0].Rank);
                if (pairIndex == 8 && 
                    ((dealerUpcard.Rank == Card.Ranks.Four && trueCount >= 6) || 
                    (dealerUpcard.Rank == Card.Ranks.Five && trueCount >= 5) || 
                    (dealerUpcard.Rank == Card.Ranks.Six && trueCount >= 4)))
                {
                    return ActionToTake.Split;
                }   
                else
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

                if (dealerUpcard.Rank == Card.Ranks.Two)
                {
                    if (total == 6 && trueCount >= 1)
                        return ActionToTake.Double; 
                }
                else if (dealerUpcard.Rank == Card.Ranks.Four)
                {
                    if (total == 8 && trueCount >= 3)
                        return ActionToTake.Double;
                }
                else if (dealerUpcard.Rank == Card.Ranks.Five || dealerUpcard.Rank == Card.Ranks.Six)
                {
                    if (total == 8 && trueCount >= 1)
                        return ActionToTake.Double;
                }
                else
                    return softStrategy[upcardIndex, total];
            }

            if(dealerUpcard.Rank == Card.Ranks.Two)
            {
                if (hand.HandValue() == 13 && trueCount <= -1)
                    return ActionToTake.Hit;
                else if (hand.HandValue() == 12 && trueCount >= 3)
                    return ActionToTake.Stand;
                else if (hand.HandValue() == 9 && trueCount >= 1)
                    return ActionToTake.Double;
            }
            else if (dealerUpcard.Rank == Card.Ranks.Three)
            {
                if (hand.HandValue() == 12 && trueCount >= 2)
                    return ActionToTake.Hit;
            }
            else if (dealerUpcard.Rank == Card.Ranks.Four)
            {
                if (hand.HandValue() == 12 && trueCount < 0)
                    return ActionToTake.Hit;
            }
            else if (dealerUpcard.Rank == Card.Ranks.Six)
            {
                if (hand.HandValue() == 8 && trueCount >= 2)
                    return ActionToTake.Double;
            }
            else if (dealerUpcard.Rank == Card.Ranks.Seven)
            {
                if (hand.HandValue() == 9 && trueCount >= 3)
                    return ActionToTake.Double;
            }
            else if (dealerUpcard.Rank == Card.Ranks.Nine)
            {
                if (hand.HandValue() == 16 && trueCount >= 4)
                    return ActionToTake.Stand;
            }
            else if ((dealerUpcard.Rank == Card.Ranks.Ten) || 
                (dealerUpcard.Rank == Card.Ranks.Jack) || 
                (dealerUpcard.Rank == Card.Ranks.Queen) || 
                (dealerUpcard.Rank == Card.Ranks.King))
            {
                if (hand.HandValue() == 16 && trueCount > 0)
                    return ActionToTake.Stand;
                else if (hand.HandValue() == 15 && trueCount >= 4)
                    return ActionToTake.Stand;
                else if (hand.HandValue() == 10 && trueCount >= 4)
                    return ActionToTake.Double;
            }
            else if (dealerUpcard.Rank == Card.Ranks.Ace)
            {
                if (hand.HandValue() == 11 && trueCount >= 1)
                    return ActionToTake.Double;
                else if (hand.HandValue() == 10 && trueCount >= 4)
                    return ActionToTake.Double;
            }
            else return hardStrategy[upcardIndex, hand.HandValue()];

            return hardStrategy[upcardIndex, hand.HandValue()];
        }

    }
}
