using System;
using System.Collections.Generic;

namespace App4GDW
{
    public class ScoreCard
    {
        public int SCrowID { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int FRID { get; set; }
        public int FUserID { get; set; }
        public int? FTCID { get; set; }
        public int RowNumber { get; set; }
        public string Theme { get; set; }

        public int? H01 { get; set; }
        public int? H02 { get; set; }
        public int? H03 { get; set; }
        public int? H04 { get; set; }
        public int? H05 { get; set; }
        public int? H06 { get; set; }
        public int? H07 { get; set; }
        public int? H08 { get; set; }
        public int? H09 { get; set; }
        public int? H10 { get; set; }
        public int? H11 { get; set; }
        public int? H12 { get; set; }
        public int? H13 { get; set; }
        public int? H14 { get; set; }
        public int? H15 { get; set; }
        public int? H16 { get; set; }
        public int? H17 { get; set; }
        public int? H18 { get; set; }

        public double? HOut { get; set; }
        public double? HIn { get; set; }
        public double? HTotal { get; set; }

        public bool IsSelected { get; set; }

        public List<ScoreCard> ScoreRows { get; set; }

        public virtual Round Round { get; set; }

        public virtual TeeCommonInfoes TeeCommonInfo { get; set; }
    }
}