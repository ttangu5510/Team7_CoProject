using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 캐릭터의 정보를 CSV에서 읽어와서 만드는 캐릭터 정보 객체
public class DomAthModel : MonoBehaviour
{
    public int id { get; private set; }
    public string name { get; private set; }
    public AthGrade grade { get; private set; }
    public int crutingAge { get; private set; }
    public int retireAge { get; private set; }
    public MaxPotential maxPotential { get; private set; }
    public int health { get; private set; }
    public int quickness { get; private set; }
    public int flexibility { get; private set; }
    public int technic { get; private set; }
    public int speed { get; private set; }
    public int balance { get; private set; }
    public int fatigue { get; private set; }
    public bool injured { get; private set; }
    public bool retired { get; private set; }
    
    public void Init(int athleteId)
    {
        // 여기서 Init으로 전체 파라매터를 채움
    }
    
}

public enum AthGrade
{
    Normal, Prospect, National
}

public enum MaxPotential
{
    E,C,A
}
