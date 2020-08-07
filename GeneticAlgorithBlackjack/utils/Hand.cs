﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithBlackjack.utils
{
    class Hand
    {
        public List<Card> Cards { get; set; }

        public Hand()
        {
            Cards = new List<Card>();
        }

        public void AddCard(Card card)
        {
            Cards.Add(card);
        }

        public override string ToString()
        {
            List<string> cards = new List<string>();
            foreach (var card in Cards)
                cards.Add(card.ToString());

            string hand = String.Join(",", cards);
            return hand + " = " + HandValue().ToString();
        }

        public bool IsPair()
        {
            if (Cards.Count > 2) return false;
            return (Cards[0].Rank == Cards[1].Rank);
        }

        public int HandValue()
        {
            // Retorna el mejor valor de la mano
            int highValue = 0, lowValue = 0;
            bool aceWasUsedAsHigh = false;
            foreach (var card in Cards)
            {
                if (card.Rank == Card.Ranks.Ace)
                {
                    if (!aceWasUsedAsHigh)
                    {
                        highValue += card.RankValueHigh;
                        lowValue += card.RankValueLow;
                        aceWasUsedAsHigh = true;
                    }
                    else
                    {
                        // Solo una As puede utilizarse como high, por eso todas las otras son low
                        highValue += card.RankValueLow;
                        lowValue += card.RankValueLow;
                    }

                }
                else
                {
                    //Cuando la carta no es un As, RankValueLow == RankValueHigh
                    highValue += card.RankValueLow; 
                    lowValue += card.RankValueLow;
                }
            }

            // Si el low value > 21, el high value también lo será, entonces retornamos el low, que va a ser mas cercano a 21
            if (lowValue > 21) return lowValue;

            // Si el high value > 21, retornamos el low 
            if (highValue > 21) return lowValue;

            // else, retornamos el high value, que será igual que el low amenos que haya un As.
            return highValue;
        }
    }

}

