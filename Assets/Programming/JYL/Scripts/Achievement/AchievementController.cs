using System.Collections;
using System.Collections.Generic;
using JWS;
using UnityEngine;
using Zenject;

namespace JYL
{
    public class AchievementController
    {
        private Achievement achieve;
        private AchievementState state;
        private float progress;

        public AchievementController(Achievement data, AchievementSave save)
        {
            achieve = data;
            progress = save.progress;
            state =  save.state;
        }
    }
}

