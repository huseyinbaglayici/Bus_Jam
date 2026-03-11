using System.Collections;
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
        public Queue<Vector3> PathPoints { get; set; }

        public PassengerTargetType CurrentTarget = PassengerTargetType.None;
        public Vector3 TargetWorldPosition;

        public PassengerEntity(EntityColor color, Vector2Int startPos)
        {
            GridPosition = startPos;
            Color = color;
            IsMoveable = false;
            IsTapped = false;
            PathPoints = new Queue<Vector3>();
            CurrentTarget = PassengerTargetType.None;
        }

        public void SetPath(List<Vector3> newPath)
        {
            PathPoints.Clear();
            foreach (var targetPoint in newPath)
            {
                PathPoints.Enqueue(targetPoint);
            }
        }

        public bool HasPath() => PathPoints != null && PathPoints.Count > 0;
    }
}