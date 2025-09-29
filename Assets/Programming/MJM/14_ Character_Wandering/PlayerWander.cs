using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Animator))]
public class PlayerWander : MonoBehaviour
{
    public Waypoint currentWaypoint;
    private Waypoint lastBuilding;
    private Animator animator;

    public float moveSpeed = 2f;
    private Queue<Waypoint> path = new Queue<Waypoint>();
    private bool isWaiting = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (currentWaypoint != null && currentWaypoint.isBuilding)
        {
            StartCoroutine(WaitAtBuilding()); // 시작이 건물이면 바로 대기
        }
    }

    void Update()
    {
        if (isWaiting || path.Count == 0) return;

        Waypoint target = path.Peek();
        Vector3 targetPos = target.transform.position;

        // 이동
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        // 회전
        Vector3 direction = (targetPos - transform.position).normalized;
        if (direction.magnitude > 0.1f)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 10f);
            animator.SetBool("isWalking", true);
        }

        // 도착 체크
        if (Vector3.Distance(transform.position, targetPos) < 0.1f)
        {
            currentWaypoint = target;
            path.Dequeue();

            if (path.Count == 0 && currentWaypoint.isBuilding)
            {
                lastBuilding = currentWaypoint;
                StartCoroutine(WaitAtBuilding());
            }
        }
    }

    IEnumerator WaitAtBuilding()
    {
        isWaiting = true;
        animator.SetBool("isWalking", false);
        ToggleVisibility(false);

        float waitTime = Random.Range(2f, 5f);
        yield return new WaitForSeconds(waitTime);

        ToggleVisibility(true);
        isWaiting = false;

        ChooseNextBuilding();
    }

    void ChooseNextBuilding()
    {
        // 씬에 있는 모든 건물 웨이포인트 수집
        Waypoint[] all = GameObject.FindObjectsOfType<Waypoint>();
        List<Waypoint> buildingCandidates = new List<Waypoint>();

        foreach (var wp in all)
        {
            if (!wp.isBuilding) continue;
            if (wp == currentWaypoint) continue;   // 자기 자신 제외
            if (wp == lastBuilding) continue;      // 직전 건물 제외
            buildingCandidates.Add(wp);
        }

        if (buildingCandidates.Count == 0) return;

        // 랜덤한 목표 건물 선택
        Waypoint targetBuilding = buildingCandidates[Random.Range(0, buildingCandidates.Count)];

        // 최단 경로 탐색
        List<Waypoint> newPath = FindShortestPath(currentWaypoint, targetBuilding);
        if (newPath != null && newPath.Count > 0)
        {
            path = new Queue<Waypoint>(newPath);
            Debug.Log($"{name}: {currentWaypoint.name} → {targetBuilding.name} 경로 찾음, 경유 {newPath.Count}개");
        }
        else
        {
            Debug.LogWarning($"{name}: {targetBuilding.name} 까지 경로를 찾을 수 없음!");
        }
    }

    List<Waypoint> FindShortestPath(Waypoint start, Waypoint goal)
    {
        Queue<Waypoint> queue = new Queue<Waypoint>();
        Dictionary<Waypoint, Waypoint> cameFrom = new Dictionary<Waypoint, Waypoint>();

        queue.Enqueue(start);
        cameFrom[start] = null;

        while (queue.Count > 0)
        {
            Waypoint current = queue.Dequeue();

            if (current == goal)
            {
                // 경로 복원
                List<Waypoint> path = new List<Waypoint>();
                while (current != null)
                {
                    path.Insert(0, current);
                    current = cameFrom[current];
                }
                path.RemoveAt(0); // 첫 번째(start)는 제거
                return path;
            }

            foreach (var neighbor in current.neighbors)
            {
                if (!cameFrom.ContainsKey(neighbor))
                {
                    queue.Enqueue(neighbor);
                    cameFrom[neighbor] = current;
                }
            }
        }
        return null;
    }

    void ToggleVisibility(bool visible)
    {
        foreach (var r in GetComponentsInChildren<Renderer>())
            r.enabled = visible;
    }
}
