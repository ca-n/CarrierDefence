using System;
using UnityEngine.Events;

namespace Events
{
    [Serializable]
    public class WaveChangedEvent : UnityEvent<int, int>
    {
        
    }
}