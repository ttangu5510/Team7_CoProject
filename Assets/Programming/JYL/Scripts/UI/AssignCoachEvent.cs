using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JYL
{
    public struct AssignCoachEvent
    {
        public int SlotNumber { get; }
        public int CoachId { get; }

        public AssignCoachEvent(int slotNumber, int id)
        {
            SlotNumber = slotNumber;
            CoachId = id;
        }
    }
}

