using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Zenject;
using EditorAttributes;

namespace SHG
{

  public class FacilityComponent : MonoBehaviour
  {
    const float RAYCAST_MAX_DIST = 100f;

    [Inject]
    IFacilitiesController facilitiesController; 

    public IFacility.FacilityType FacilityType => this.facilityType;

    [SerializeField]
    IFacility.FacilityType facilityType;
    [SerializeField] [Required]
    Transform model;
    int layer;

    // Start is called before the first frame update
    void Start()
    {
    }
  }
}
