using System;
using Types;
using UnityEngine.Events;

namespace Events
{
    [Serializable]
    public class GameStateChangedEvent : UnityEvent<GameState>
    {
        
    }
}