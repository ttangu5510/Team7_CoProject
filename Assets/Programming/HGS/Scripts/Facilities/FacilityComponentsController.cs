using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Zenject;
using EditorAttributes;

namespace SHG
{
  public class FacilityComponentsController : MonoBehaviour
  {
    const float RAYCAST_MAX_DIST = 100f;

    [Inject]
    TouchController touchController;
    [Inject]
    IFacilitiesController facilitiesController; 
    int layer;
    FacilityComponent touchedFacility;

    void Start()
    {
      this.layer = (1 << LayerMask.NameToLayer("Facility"));
      this.touchController.OnTouchDown
        .Subscribe(touch => 
          this.touchedFacility = this.GetTouchedFacility(touch.position));
      this.touchController.OnTouchUp
        .Where(_ => this.touchedFacility != null &&
          this.facilitiesController.Selected.Value == null)
        .Subscribe(touch => {
          var facility = this.GetTouchedFacility(touch.position);
          if (this.touchedFacility == facility) {
            this.facilitiesController.SelectFacilityType(facility.FacilityType) ;
          }});

    }

    FacilityComponent GetTouchedFacility(Vector2 pos)
    {
      var ray = Camera.main.ScreenPointToRay(pos);
      if (Physics.Raycast(
        ray: ray, 
        maxDistance: RAYCAST_MAX_DIST, 
        layerMask: this.layer,
        hitInfo: out RaycastHit hitInfo)) {
        return (hitInfo.collider.GetComponent<FacilityComponent>());
      }
      else {
        return (null);
      }
    }
  }
}
