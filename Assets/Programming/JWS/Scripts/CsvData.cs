using System;
using JYL;

namespace JWS
{
    public class AthleteCsvData
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
        
        public AthleteCsvData(string[] row)
        {
            if (row.Length < 11)
                throw new ArgumentException("Athlete CSV row 데이터 오류");

            ID = int.Parse(row[0]);
            Name = row[1];
            Affiliation = Enum.Parse<AthleteAffiliation>(row[2]);
            Grade = Enum.Parse<AthleteGrade>(row[3]);
            RecruitAge = int.Parse(row[4]);
            Health = int.Parse(row[5]);
            Quickness = int.Parse(row[6]);
            Flexibility = int.Parse(row[7]);
            Technic = int.Parse(row[8]);
            Speed = int.Parse(row[9]);
            Balance = int.Parse(row[10]);
        }
    }

    public class CoachCsvData
    {
        public int ID;
        public string Name;
        public CoachGrade Grade;
        public int Age;

        public CoachCsvData(string[] row)
        {
            if (row.Length < 4)
                throw new ArgumentException("Coach CSV row 데이터 오류");

            ID = int.Parse(row[0]);
            Name = row[1];
            Grade = Enum.Parse<CoachGrade>(row[2]);
            Age = int.Parse(row[3]);
        }
    }
}