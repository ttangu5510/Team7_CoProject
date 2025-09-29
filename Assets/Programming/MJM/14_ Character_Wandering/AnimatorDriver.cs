using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatorDriver : MonoBehaviour
{
    public string speedParam = "Speed";
    public string walkingParam = "isWalking";
    public float smoothing = 10f;

    Vector3 _prevPos;
    Animator _anim;
    float _speed;

    void Awake()
    {
        _anim = GetComponent<Animator>();
        _prevPos = transform.position;
    }

    void Update()
    {
        var delta = (transform.position - _prevPos) / Mathf.Max(Time.deltaTime, 0.0001f);
        float rawSpeed = new Vector3(delta.x, 0f, delta.z).magnitude; // 지면 속도
        _speed = Mathf.Lerp(_speed, rawSpeed, Time.deltaTime * smoothing);

        if (_anim.HasParameter(speedParam)) _anim.SetFloat(speedParam, _speed);
        if (_anim.HasParameter(walkingParam)) _anim.SetBool(walkingParam, _speed > 0.05f);

        _prevPos = transform.position;
    }
}

static class AnimatorExt
{
    public static bool HasParameter(this Animator anim, string name)
    {
        foreach (var p in anim.parameters) if (p.name == name) return true;
        return false;
    }
}
