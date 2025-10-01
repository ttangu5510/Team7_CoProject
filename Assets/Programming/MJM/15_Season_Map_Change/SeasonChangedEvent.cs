using JWS;

public class SeasonChangedEvent
{
    public int YearCycle;
    public Season NewSeason;

    public SeasonChangedEvent(int year, Season season)
    {
        YearCycle = year;
        NewSeason = season;
    }
}
