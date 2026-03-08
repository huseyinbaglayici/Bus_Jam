using _Scripts.Runtime.Enums;
using UnityEngine;

namespace _Scripts.Runtime.Gameplay.Entities.Passenger
{

    public enum PassengerTargetType
    {
        None,
        Bus,
        Line
    }
    
    [System.Serializable]
    public class PassengerEntity
    {
        public EntityColor Color;
        public Vector2Int GridPosition;

        public bool IsMoveable;
        public bool IsTapped;

        public PassengerTargetType CurrentTarget = PassengerTargetType.None;
        public Vector3 TargetWorldPosition;
        
        public PassengerEntity(EntityColor color, Vector2Int gridPosition)
        {
            GridPosition = gridPosition;
            Color = color;
            IsMoveable = false;
            IsTapped = false;
            CurrentTarget = PassengerTargetType.None;
            
        }
    }
}