using System.Collections.Generic;
using _Scripts.Runtime.Data.ValueObjects;
using UnityEngine;

namespace _Scripts.Runtime.Data.UnityObjects
{
    [CreateAssetMenu(fileName = "New_LevelData", menuName = "BusJam/Level Data")]
    public class LevelDataSO : ScriptableObject
    {
        public int Rows;
        public int Cols;
        public int PassengerLineCapacity;
        public int Time;

        public List<BusLineSaveData> BusSequence;
        public List<CellSaveData> GridCells;
    }
}