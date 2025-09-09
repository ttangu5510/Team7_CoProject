using System;
using JYL;

namespace JWS
{
    public class DomAthleteCsvData
    {
        public readonly int ID;
        public readonly string Name;
        public readonly AthleteAffiliation Affiliation;
        public readonly AthleteGrade Grade;
        public readonly int RecruitAge;
        public readonly int Health;
        public readonly int Quickness;
        public readonly int Flexibility;
        public readonly int Technic;
        public readonly int Speed;
        public readonly int Balance;
        
        public DomAthleteCsvData(string[] row)
        {
            if (row.Length < 18)
                throw new ArgumentException("Dom Athlete CSV row 데이터 오류");

            ID = int.Parse(row[0]);
            Name = row[1];
            Affiliation = Enum.Parse<AthleteAffiliation>(row[2]);
            Grade = Enum.Parse<AthleteGrade>(row[5]);
            RecruitAge = int.Parse(row[3]);
            Health = int.Parse(row[6]);
            Quickness = int.Parse(row[7]);
            Flexibility = int.Parse(row[8]);
            Technic = int.Parse(row[9]);
            Speed = int.Parse(row[10]);
            Balance = int.Parse(row[11]);
        }
    }

    public class CoachCsvData
    {
        public readonly int ID;
        public readonly string Name;
        public readonly CoachGrade Grade;
        public readonly int Age;

        public CoachCsvData(string[] row)
        {
            if (row.Length < 6)
                throw new ArgumentException("Coach CSV row 데이터 오류");

            ID = int.Parse(row[0]);
            Name = row[1];
            Grade = Enum.Parse<CoachGrade>(row[2]);
            Age = int.Parse(row[3]);
        }
    }
    
    public class ForAthleteCsvData
    {
        public readonly int ID;
        public readonly string Name;
        public readonly AthleteAffiliation Affiliation;
        public readonly int Health;
        public readonly int Quickness;
        public readonly int Flexibility;
        public readonly int Technic;
        public readonly int Speed;
        public readonly int Balance;
        
        public ForAthleteCsvData(string[] row)
        {
            if (row.Length < 10)
                throw new ArgumentException("For Athlete CSV row 데이터 오류");

            ID = int.Parse(row[0]);
            Name = row[1];
            Affiliation = Enum.Parse<AthleteAffiliation>(row[2]);
            Health = int.Parse(row[3]);
            Quickness = int.Parse(row[4]);
            Flexibility = int.Parse(row[5]);
            Technic = int.Parse(row[6]);
            Speed = int.Parse(row[7]);
            Balance = int.Parse(row[8]);
        }
    }
}