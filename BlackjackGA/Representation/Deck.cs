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
        
        public Deck(int numDecksToUse)
        {
            numDecks = numDecksToUse;
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
            return Cards[currentCard++];
        }


        public int CardsRemaining {
            get
            {
                return Cards.Count - currentCard;
            }
        }

        public void Shuffle()
        {
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
