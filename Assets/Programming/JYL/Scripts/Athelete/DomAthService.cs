using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JYL
{
    public class DomAthService : MonoBehaviour
    {
        public DomAthRepository rp;

        private Test_JYL_SaveManager saveM;
        

        public void Init() // 초기화
        {
            rp = new();
            rp.Init();
        }

        #region 선수 목록
        public List<DomAthEntity> GetAllAthleteList() // 국내 전체 선수 목록 뽑아가기
        {
            return rp.athleteDict.Values.ToList();
        }

        public List<DomAthEntity> GetRecruitedAthleteList() // 영입된 선수 목록 뽑아가기
        {
            return rp.recruitDict.Values.ToList();
        }
        #endregion
        
        #region 선수 영입, 방출
        // 은퇴는 이벤트 처리 필요
        public void RecruitAthlete(string athleteName) // 새로운 선수를 영입할 때 사용하는 함수
        {
            DomAthEntity athlete = rp.athleteDict[athleteName]; // 딕셔너리에서 찾아옴
            athlete.Recruit(); // isRecruited true로 변경
            rp.UpdateRecruitDict(); // 딕셔너리 최신화
            saveM.RecruitAthlete(athlete); // 선수 세이브 객체 생성
        }

        public void OutAthlete(string athleteName) // 선수 방출할 때 쓰는 함수
        {
            DomAthEntity athlete = rp.recruitDict[athleteName];
            athlete.OutAthlete();
            rp.CreateAthleteList(); // 해당 선수의 성장 기록 말소를 위한 딕셔너리 최신화
            rp.UpdateRecruitDict(); // 딕셔너리 최신화
            saveM.OutAthlete(athlete); // 선수 세이브 객체 삭제
        }
        #endregion

        #region 선수 강화
        public void TrainAthlete(in string athleteName, in Ability status, int amount = 1, int coach = 0)
        { //선수 훈련 함수. 정해진 파라매터만 수행 가능 (기획안의 루틴에 따름). 부상이면 선수 강화 함수 수행하면 안됨
            switch (status)
            {
                case Ability.Health :
                case Ability.Quickness :
                case Ability.Flexibility :
                case Ability.Balance :
                    // 선수를 딕셔너리에서 찾고 훈련
                    DomAthEntity athlete = rp.recruitDict[athleteName];
                    if(athlete != null) athlete.TrainAthlete(status, amount, coach);
                    
                    // 선수 세이브 객체 최신화
                    saveM.curSave.FindAthlete(athlete).UpdateStatus(athlete);
                    break;
                default:
                    Debug.LogWarning($"잘못된 파라매터 입력{status}");
                    break;
            }
        }
        #endregion
        
        
        #region 선수 회복
        // 선수가 회복하는 함수. 파라매터만 변경 하는 것이기 때문에, 결과 처리는 UI에서 필요함. 마찬가지로, 부상 상태가 아니면 수행 못하게 해야함
        public void RecoverAthlete(in string athleteName, int  amount = 1)
        {
            DomAthEntity athlete = rp.recruitDict[athleteName];
            if (athlete.curState == AthleteState.Injured && athlete.leftInjury > 0)
            {
                athlete.RecoverAthlete(amount); // 리커버리. 부상을 한 턴 감소.
                saveM.curSave.FindAthlete(athlete).UpdateStatus(athlete); // 선수 세이브 객체 최신화
                Debug.Log($"{athlete.name} 부상 회복");
            }
            else
            {
                Debug.LogWarning($"해당 선수는 부상당한 상태가 아님!{athlete.name}_isInjured={athlete.curState == AthleteState.Injured}");
            }
        }
        #endregion
    }
}
