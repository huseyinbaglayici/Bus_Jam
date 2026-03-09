using _Scripts.Runtime.Enums;
using UnityEngine;

namespace _Scripts.Runtime.Data.ValueObjects
{
    [System.Serializable]
    public struct CellSaveData
    {
        public Vector2Int coordinates;
        public CellType type;
        public EntityColor color;
        public OccupantType occupant;
    }
}