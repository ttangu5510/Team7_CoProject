using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(Waypoint))]
public class WaypointEditor : Editor
{
    void OnSceneGUI()
    {
        Waypoint wp = (Waypoint)target;

        // 다른 모든 Waypoint들을 씬에서 찾음
        Waypoint[] all = GameObject.FindObjectsOfType<Waypoint>();

        Handles.color = Color.cyan;
        foreach (var other in all)
        {
            if (other == wp) continue;

            if (Handles.Button(other.transform.position, Quaternion.identity, 0.2f, 0.2f, Handles.SphereHandleCap))
            {
                Undo.RecordObject(wp, "Add Neighbor");
                if (!wp.neighbors.Contains(other))
                    wp.neighbors.Add(other);

                Undo.RecordObject(other, "Add Neighbor");
                if (!other.neighbors.Contains(wp))
                    other.neighbors.Add(wp);

                EditorUtility.SetDirty(wp);
                EditorUtility.SetDirty(other);
            }
        }
    }

    [MenuItem("GameObject/Create Waypoint %#w")]
    static void CreateWaypoint()
    {
        GameObject go = new GameObject("Waypoint");
        go.AddComponent<Waypoint>();
        Undo.RegisterCreatedObjectUndo(go, "Create Waypoint");
        Selection.activeGameObject = go;
    }
}
