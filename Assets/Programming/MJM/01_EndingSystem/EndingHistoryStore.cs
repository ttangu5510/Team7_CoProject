using UnityEngine;
using System.Collections.Generic;

public static class EndingHistoryStore
{
    private const string KEY = "ENDING_HISTORY_V1";
    private const int MAX_RECORDS = 20;

    [System.Serializable]
    private class Wrapper { public List<EndingResult> list = new List<EndingResult>(); }

    public static void Save(EndingResult result)
    {
        var w = LoadAllWrapper();
        w.list.Insert(0, result);
        if (w.list.Count > MAX_RECORDS) w.list.RemoveRange(MAX_RECORDS, w.list.Count - MAX_RECORDS);
        PlayerPrefs.SetString(KEY, JsonUtility.ToJson(w));
        PlayerPrefs.Save();
    }

    public static List<EndingResult> LoadAll()
    {
        return LoadAllWrapper().list;
    }

    private static Wrapper LoadAllWrapper()
    {
        if (!PlayerPrefs.HasKey(KEY)) return new Wrapper();
        var json = PlayerPrefs.GetString(KEY);
        var w = JsonUtility.FromJson<Wrapper>(json);
        return w ?? new Wrapper();
    }
}
