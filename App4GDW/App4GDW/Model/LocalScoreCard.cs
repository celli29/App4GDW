using System;
namespace App4GDW
{
    public class LocalScoreCard
    {
        public LocalScoreCard()
        {
        }

        public int Hole { get; set; }
        public int Par { get; set; }
        public int Handicap { get; set; }
        public int Distance { get; set; }

        public int Score { get; set; }
        public int AltScore { get; set; }
        public int Putt { get; set; }
        public int Fairway { get; set; }
        public int GIR { get; set; }
        public int Chip { get; set; }
        public int Pitch { get; set; }
        public int GSBunker { get; set; }
        public int Penalty { get; set; }
    }
}
