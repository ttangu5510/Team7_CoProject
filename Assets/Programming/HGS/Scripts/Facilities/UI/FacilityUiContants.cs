using System.Collections.Generic;

namespace SHG
{
  using FacilityType = IFacility.FacilityType;

  public static class FacilityUiConstants
  {
    public static readonly Dictionary<FacilityType, string[]> TAB_BUTTON_TEXTS = new Dictionary<FacilityType, string[]>{
      {
        FacilityType.Accomodation, 
          new string[] {
            "시설 정보"
          }
      },
        {
          FacilityType.Lounge, 
          new string[] {
            "시설 정보", "휴식"
          }
        },
        {
          FacilityType.TrainingCenter, 
          new string[] {
            "시설 정보", "훈련", "특훈", "코치"
          }
        },
        {
          FacilityType.MedicalCenter, 
          new string[] {
            "시설 정보", "치료실"
          }
        },
        {
          FacilityType.ScoutCenter, 
          new string[] {
            "시설 정보", "선수 영입", "코치 영입"
          }
        },
    };
  }
}
