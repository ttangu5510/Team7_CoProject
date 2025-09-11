using System.IO;
using UnityEngine;

namespace JWS
{
    public static class SaveSystem
    {
        private static string saveFile = Path.Combine(Application.persistentDataPath, "save.json");

        // 데이터 저장 (자동/수동 모두 이걸 호출)
        public static void SaveData(SaveData data)
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(saveFile, json);
            Debug.Log("저장 완료: " + saveFile);
        }

        // 데이터 불러오기
        public static SaveData LoadData()
        {
            if (File.Exists(saveFile))
            {
                string json = File.ReadAllText(saveFile);
                SaveData data = JsonUtility.FromJson<SaveData>(json);
                Debug.Log("불러오기 완료");
                return data;
            }
            else
            {
                Debug.LogWarning("저장 파일 없음, 새 데이터 생성");
                return new SaveData();
            }
        }

        // 데이터 삭제
        public static void DeleteData()
        {
            if (File.Exists(saveFile))
            {
                File.Delete(saveFile);
                Debug.Log("저장 데이터 삭제됨");
            }
        }

        // 수동 저장 (UI 버튼에 연결)
        public static void ManualSave(SaveData data)
        {
            Debug.Log("사용자 요청으로 수동 저장 실행");
            SaveData(data);
        }
    }
}