namespace JYL
{
    public struct AthleteRetiredEvent
    {

        // 만준 추가 코드
        public int athleteId { get; }
        public string athleteName { get; }
        public AthleteAffiliation affiliation { get; }

        // public AthleteRetiredEvent(string athleteName,  AthleteAffiliation affiliation)
        // {
        //     this.athleteName = athleteName;
        //     this.affiliation = affiliation;
        // }

        // 기존 호환: name + affiliation 기반
        public AthleteRetiredEvent(string athleteName, AthleteAffiliation affiliation)
        {
            this.athleteId = -1;
            this.athleteName = athleteName;
            this.affiliation = affiliation;
        }

        // 만준 추가 코드
        public AthleteRetiredEvent(int athleteId)
        {
            this.athleteId = athleteId;
            this.athleteName = null;
            this.affiliation = default;
        }


    }
}