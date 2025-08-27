using UnityEngine;
using System.Collections.Generic;

public static class EndingHistoryStore
{
    private const string KEY = "ENDING_HISTORY_V1";  // PlayerPrefs에 저장될 키 이름 (데이터 식별용)
    private const int MAX_RECORDS = 20;              // 최대 저장 개수 (최근 엔딩 기록만 n개까지 보관)

    [System.Serializable]
    private class Wrapper { public List<EndingResult> list = new List<EndingResult>(); }    // JsonUtility에서 직렬화하기 위해 필요한 Wrapper 클래스


    /// <summary>
    /// 엔딩 결과를 저장한다.
    /// - 최근 기록이 가장 앞에 오도록 삽입
    /// - 최대 개수를 초과하면 오래된 기록 삭제
    /// - PlayerPrefs에 JSON 문자열로 직렬화하여 저장
    /// </summary>
    public static void Save(EndingResult result)
    {
        var w = LoadAllWrapper();    // 기존 기록 불러오기
        w.list.Insert(0, result);    // 가장 앞(최신)에 삽입
        if (w.list.Count > MAX_RECORDS) w.list.RemoveRange(MAX_RECORDS, w.list.Count - MAX_RECORDS);    // 최대 개수 초과 시
        PlayerPrefs.SetString(KEY, JsonUtility.ToJson(w));      // JSON 직렬화 후 저장
        PlayerPrefs.Save();                                      // 즉시 저장
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
