using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 어디서나 참조 가능한 폴더(예: Scripts/Domain/Events)에 두세요.
public readonly struct AthleteFiredEvent
{
    public readonly string name;
    public AthleteFiredEvent(string name) { this.name = name; }
}
