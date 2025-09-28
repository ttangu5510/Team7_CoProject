using UnityEngine;

public class SimplePingPongMover : MonoBehaviour
{
    public Transform a, b;
    public float speed = 2f;
    Transform _target;

    void Start() { _target = b; }

    void Update()
    {
        if (_target == null) return;
        transform.position = Vector3.MoveTowards(transform.position, _target.position, speed * Time.deltaTime);
        Vector3 dir = (_target.position - transform.position).normalized;
        if (dir.sqrMagnitude > 0.001f)
            transform.forward = Vector3.Lerp(transform.forward, dir, Time.deltaTime * 10f);

        if (Vector3.Distance(transform.position, _target.position) < 0.05f)
            _target = _target == a ? b : a;
    }
}
