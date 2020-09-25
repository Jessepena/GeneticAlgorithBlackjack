using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackjackGA.Representation
{
    class Card
    {
        public enum Suits { Hearts, Spades, Clubs, Diamonds };
        public enum Ranks { Two = 2, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King, Ace };

        public static int HighestRankIndex = 9;

        public Ranks Rank { get; set; }
        public Suits Suit { get; set; }

        private static List<Ranks> rankList;
        private static List<Suits> suitList;

        public Card(Ranks rankValue, Suits suit)
        {
            Rank = rankValue;
            Suit = suit;
        }

        public static List<Ranks> ListOfRanks
        {
            get
            {
                if (rankList != null) return rankList;

                var result = new List<Ranks>();
                var ranks = Enum.GetValues(typeof(Ranks));
                foreach (var rank in ranks)
                    result.Add((Ranks)rank);
                rankList = result;
                return result;
            }
        }

        public static List<Suits> ListOfSuits
        {
            get
            {
                if (suitList != null) return suitList;

                var suits = Enum.GetValues(typeof(Suits));
                var result = new List<Suits>();
                foreach (var suit in suits)
                    result.Add((Suits)suit);
                suitList = result;
                return result;
            }
        }

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
