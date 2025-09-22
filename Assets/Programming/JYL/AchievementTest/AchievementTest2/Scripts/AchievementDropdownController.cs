using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace JYL.AchievementTest02
{
    [RequireComponent(typeof(TMP_Dropdown))]
    public class AchievementDropdownController : MonoBehaviour
    {
        private TMP_Dropdown dropdown;

        private TMP_Dropdown Dropdown
        {
            get
            {
                if (dropdown == null)
                {
                    dropdown = GetComponent<TMP_Dropdown>();
                }
                return dropdown;
            }
        }

        public UnityAction<Achievements> onValueChanged;

        private void Start()
        {
            Dropdown.onValueChanged.AddListener(HandleDropdownValueChanged);
        }
        
        [ContextMenu("UpdateOptions()")]
        public void UpdateOptions()
        {
            Dropdown.options.Clear();
            var values = Enum.GetValues(typeof(Achievements));
            foreach (Achievements achievement in values)
            {
                Dropdown.options.Add(new TMP_Dropdown.OptionData(achievement.ToString()));
            }
            Dropdown.RefreshShownValue();
        }

        private void HandleDropdownValueChanged(int value)
        {
            if (onValueChanged != null)
            {
                onValueChanged.Invoke((Achievements)value);
            }
        }
    }
}

