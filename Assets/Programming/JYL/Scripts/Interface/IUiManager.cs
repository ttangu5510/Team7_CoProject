using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JYL
{
    public interface IUiManager
    {
        // 패널
        public void OpenPanel(string rawKey, bool toggleIfSame = true);
        public void CloseAllPanels();
        public bool RegisterPanel(string rawKey, GameObject panel);
        public void UnregisterPanel(string rawKey);

        // 팝업
        public GameObject ShowPopup(string rawKey, object initData = null);
        //public GameObject ShowPopup(string rawKey, bool modal = true, object initData = null);
        public void CloseTopPopup();
        public void CloseSpecificPopup(GameObject popup);
    
        // 토스트
        public void ShowToast(string msg);
    }
}
