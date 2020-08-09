using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithBlackjack.representation
{
    class Card
    {
        public enum Suits { Hearts, Spades, Clubs, Diamonds };
        public enum Ranks { Two = 2, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King, Ace };

        public Ranks Rank { get; set; }
        public Suits Suit { get; set; }

        public int RankValueHigh
        {
            get
            {
                switch (Rank)
                {
                    case Ranks.Ace:
                        return 11;

                    case Ranks.King:
                    case Ranks.Queen:
                    case Ranks.Jack:
                    case Ranks.Ten:
                        return 10;

                    default:
                        return (int)Rank;
                }
            }
        }

        public int RankValueLow
        {
            get
            {
                switch (Rank)
                {
                    case Ranks.Ace:
                        return 1;

                    case Ranks.King:
                    case Ranks.Queen:
                    case Ranks.Jack:
                    case Ranks.Ten:
                        return 10;

                    default:
                        return (int)Rank;
                }
            }
        }

        public static string RankString(Ranks rank)
        {
            var rankChars = "23456789TJQKA".ToCharArray();
            return rankChars[((int)rank)-2].ToString();
        }

        public override string ToString()
        {
            return RankString(Rank) + Suit;
        }
    }

}
