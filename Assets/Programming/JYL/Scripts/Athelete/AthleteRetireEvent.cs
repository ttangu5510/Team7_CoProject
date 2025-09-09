namespace JYL
{
    public struct AthleteRetiredEvent
    {
        public string athleteName { get; }
        public AthleteAffiliation affiliation { get; }

        public AthleteRetiredEvent(string athleteName,  AthleteAffiliation affiliation)
        {
            this.athleteName = athleteName;
            this.affiliation = affiliation;
        }
    }
}