using System.Collections.Generic;
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
        public PassengerTargetType CurrentTarget;
        public Vector3 TargetWorldPosition;
        public Queue<Vector3> PathPoints { get; private set; }

        public PassengerEntity(EntityColor color, Vector2Int startPos)
        {
            Color = color;
            GridPosition = startPos;
            PathPoints = new Queue<Vector3>();
        }

        public void SetPath(List<Vector3> newPath)
        {
            PathPoints.Clear();
            foreach (var point in newPath)
                PathPoints.Enqueue(point);
        }

        public bool HasPath() => PathPoints.Count > 0;
    }
}