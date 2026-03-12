using System.Collections.Generic;
using _Scripts.Runtime.Data.ValueObjects;
using UnityEngine;

namespace _Scripts.Runtime.Data.UnityObjects
{
    [CreateAssetMenu(fileName = "ColorDatabase", menuName = "BusJam/Color Database")]
    public class ColorDatabaseSO : ScriptableObject
    {
        public List<EntityColorData> Colors;
    }
}