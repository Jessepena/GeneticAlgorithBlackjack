using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithBlackjack.Representation
{
    class Deck
    {
        private int currentCard = 0;
        private List<Card> Cards;
        private int numDecks;

        public Deck(int numDecksToUse)
        {
            numDecks = numDecksToUse;
        } 
    }

}
