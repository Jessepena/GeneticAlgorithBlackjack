using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackjackGA.Utils;

namespace BlackjackGA.Representation
{
    class Deck
    {
        private int currentCard = 0;
        private List<Card> Cards;
        private int numDecks;
        private bool countingCard;
        private TestConditions testConditions;
        public int runningCount = 0;
        public int trueCount = 0;

        public Deck(int numDecksToUse)
        {
            numDecks = numDecksToUse;
            testConditions = new TestConditions();
            CreateRandomDeck();
        } 

        private void CreateRandomDeck()
        {
            Cards = new List<Card>(52 * numDecks);

            for (int i = 0; i < numDecks; i++)
                foreach (var rank in Card.ListOfRanks)
                    foreach (var suit in Card.ListOfSuits)
                    {
                        var card = new Card(rank, suit);
                        Cards.Add(card);
                    }

            Shuffle();
        }

        public Card DealCard()
        {
            ShuffleIfNeeded();
            CountIfNeeded();
            return Cards[currentCard++];
        }

        private void CountIfNeeded()
        {
            if(testConditions.CountingCards)
            {
                runningCount += (int)Cards[currentCard].CountingValue;
                double remainingDecks = ((testConditions.NumDecks * 52) - currentCard) / 52;
                trueCount = (int)Math.Round(runningCount / Math.Round(remainingDecks));
            }      
        }

        public void ForceNextCardToBe(Card.Ranks rank)
        {
            // esta función se usa cuando se desea proporcionar las manos.
            // compensando el hecho de que los pares y las soft hands no aparecen muy a seguido,
            // y talvez queramos forzar que se reparta una carta en específico.

            // primero buscamos el rank en las cartas restantes
            int foundAt = -1;
            for (int i = currentCard; i < Cards.Count; i++)
                if (Cards[i].Rank == rank)
                {
                    foundAt = i;
                    break;
                }

            // en caso de que no se encuentre, nos vamos al principio del deck y empezamos de nuevo
            if (foundAt == -1)
            {
                for (int i = 0; i < currentCard; i++)
                    if (Cards[i].Rank == rank)
                    {
                        foundAt = i;
                        break;
                    }
            }

            // cambiamos la carta con la próxima carta a repartir
            Card temp = Cards[foundAt];
            Cards[foundAt] = Cards[currentCard];
            Cards[currentCard] = temp;
        }

        public void EnsureNextCardIsnt(Card.Ranks rank)
        {
            // similar a ForceNextCardToBe, en este caso asegurarnos de que la carta no sea una en específico
            while (Cards[currentCard].Rank == rank)
            {
                currentCard++;
                ShuffleIfNeeded();
            }
        }


        public int CardsRemaining {
            get
            {
                return Cards.Count - currentCard;
            }
        }

        public void Shuffle()
        {
            runningCount = 0;
            trueCount = 0;
            // Usamos el algoritmo Fisher-Yates para hacer shuffle
            int start = Cards.Count - 1;
            var randomizer = new Randomizer();
            for (int i = start; i > 1; i--)
            {
                int swapWith = randomizer.Lesser(i);

                Card hold = Cards[i];
                Cards[i] = Cards[swapWith];
                Cards[swapWith] = hold;
            }
            currentCard = 0;
        }

        public void ShuffleIfNeeded()
        {
            if (CardsRemaining < 20)
                Shuffle();
        }
    }

}
