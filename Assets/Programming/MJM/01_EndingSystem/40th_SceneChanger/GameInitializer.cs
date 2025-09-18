using SHG;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    [SerializeField] private TimeFlowWatcher timeFlowWatcher;
    [SerializeField] private TimeFlowTester timeFlowTester;

    private TimeFlowController controller;

    void Start()
    {
        controller = new TimeFlowController(1, 1);

        timeFlowWatcher.Initialize(controller);
        timeFlowTester.Initialize(controller); // ← 여기서 테스트 기능 연결!
    }
}
