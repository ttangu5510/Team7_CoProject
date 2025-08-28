using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using JWS;

public static class CsvReader
{
    public static List<DomAthleteCsvData> ReadAthletes(string fileName)
    {
        var rows = Read(fileName);
        if (rows == null) return null;

        List<DomAthleteCsvData> list = new();
        foreach (var row in rows.Skip(1)) // 헤더 스킵
            list.Add(new DomAthleteCsvData(row));
        return list;
    }

    public static List<CoachCsvData> ReadCoaches(string fileName)
    {
        var rows = Read(fileName);
        if (rows == null) return null;

        List<CoachCsvData> list = new();
        foreach (var row in rows.Skip(1)) // 헤더 스킵
            list.Add(new CoachCsvData(row));
        return list;
    }

    private static List<string[]> Read(string fileName)
    {
        TextAsset csvFile = Resources.Load<TextAsset>($"CSV/{fileName}");
        if (csvFile == null)
        {
            Debug.LogError($"CSV 파일을 찾을 수 없음: {fileName}");
            return null;
        }

        List<string[]> rows = new();
        string[] lines = csvFile.text.Split('\n');
        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            rows.Add(line.Trim().Split(','));
        }
        return rows;
    }
}
