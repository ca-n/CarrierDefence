using System;
using UnityEngine.Events;

namespace Events
{
    [Serializable]
    public class ScoreUpdatedEvent : UnityEvent<int>
    {
        
    }
}