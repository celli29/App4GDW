using System;
namespace App4GDW
{
    public class Course
    {
        public Course()
        {
        }

        public int GCID { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string Name { get; set; }
        public string SubCourseName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }

        public int NHole { get; set; }
        public int Pars { get; set; }
        public string Tee1 { get; set; }
        public string Tee2 { get; set; }
        public string Tee3 { get; set; }
        public string Tee4 { get; set; }
        public string Tee5 { get; set; }
        public string Tee6 { get; set; }
        public string Tee7 { get; set; }
        public string Tee8 { get; set; }
        public string Tee9 { get; set; }
        public string Phone { get; set; }
        public string URL { get; set; }
    }
}
