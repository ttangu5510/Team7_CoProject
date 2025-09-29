using UnityEngine;
using System.Collections.Generic;

public class Waypoint : MonoBehaviour
{
    public List<Waypoint> neighbors = new List<Waypoint>();
    public bool isBuilding = false;

    private void OnDrawGizmos()
    {
        Gizmos.color = isBuilding ? Color.red : Color.green;
        Gizmos.DrawSphere(transform.position, 0.2f);

        if (neighbors != null)
        {
            Gizmos.color = Color.yellow;
            foreach (var n in neighbors)
            {
                if (n != null)
                    Gizmos.DrawLine(transform.position, n.transform.position);
            }
        }
    }
}
