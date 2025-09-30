namespace JYL
{
    public struct AthleteRetiredEvent
    {

        // 만준 추가 코드
        public int athleteId { get; }
        public string athleteName { get; }
        public AthleteAffiliation affiliation { get; }

        // 수정된 코드
        public AthleteRetiredEvent(string athleteName,  AthleteAffiliation affiliation, int athleteId)
        {
            this.athleteName = null;
            this.affiliation = default;
            this.athleteId = athleteId;
        }

        // 원본 코드
        //public AthleteRetiredEvent(string athleteName, AthleteAffiliation affiliation)
        //{
        //    this.athleteName = athleteName;
        //    this.affiliation = affiliation;
        //}

        // 만준 추가 코드
        //public AthleteRetiredEvent(int athleteId)
        //{
        //    this.athleteId = athleteId;
        //    this.athleteName = null;
        //    this.affiliation = default;
        //}


    }
}