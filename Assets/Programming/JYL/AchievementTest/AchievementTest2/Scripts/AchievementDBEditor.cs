using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DG.Tweening.Plugins.Core.PathCore;
using UnityEditor;
using UnityEngine;
using Path = System.IO.Path;

namespace JYL.AchievementTest02
{
    #if UNITY_EDITOR
    [CustomEditor(typeof(AchievementDatabase))]
    public class AchievementDBEditor : Editor
    {
        private string filePath = Application.dataPath + "/Programming/JYL/AchievementTest/AchievementTest2/Scripts";
        private string fileName = "Achievements.cs";

        private StringBuilder code;
        
        private AchievementDatabase database;

        private void OnEnable()
        {
            database = target as AchievementDatabase;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Generate Enum", GUILayout.Height(30)))
            {
                GenerateEnum();
            }
        }

        private void GenerateEnum()
        {
            string fullPath = Path.Combine(filePath, fileName);
            
            code = new StringBuilder("public enum Achievements{");
            foreach (Achievement achievement in database.achievements)
            {
                code.Append(achievement.id + ",");
            }

            code.Append("}");
            File.WriteAllText(fullPath, code.ToString());
            AssetDatabase.ImportAsset(fullPath);
        }
    }
    #endif
}

