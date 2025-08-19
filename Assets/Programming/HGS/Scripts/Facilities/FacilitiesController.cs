using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace SHG
{
  public class FacilitiesController : MonoBehaviour
  {
    List<IFacilityData> data;

    public FacilitiesController()
    {
      this.data = new (); 
      this.LoadData();
    }

    void LoadData()
    {

    }
  }
}
