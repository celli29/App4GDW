using System;
using System.Collections.Generic;

namespace App4GDW
{
    public class Round
    {
        public Round()
        {
            ScoreCards = new List<ScoreCard>();
        }

        public int RID { get; set; }
        public DateTime? RecordDate { get; set; }
        public DateTime? PlayDate { get; set; }
        public string UserName { get; set; }
        public DateTime? PlayStartTime { get; set; }
        public DateTime? PlayEndTime { get; set; }
        public int CourseID { get; set; }
        public int TeeID { get; set; }
        public int Score { get; set; }
        public int Putt { get; set; }
        public double GIR { get; set; }
        public double FH { get; set; }
        public int OutScore { get; set; }
        public int OutPutt { get; set; }
        public double OutFH { get; set; }
        public double OutGIR { get; set; }
        public int InScore { get; set; }
        public int InPutt { get; set; }
        public double InFH { get; set; }
        public double InGIR { get; set; }
        public Guid UserGUID { get; set; }
        public int NineHole { get; set; }
        public int ValidRound { get; set; }

        public virtual Course Course { get; set; }

        public virtual IList<ScoreCard> ScoreCards { get; set; }
    }
}