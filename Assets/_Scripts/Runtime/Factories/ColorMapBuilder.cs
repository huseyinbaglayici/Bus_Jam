using System.Collections.Generic;
using _Scripts.Runtime.Data.ValueObjects;
using _Scripts.Runtime.Enums;
using UnityEngine;

namespace _Scripts.Runtime.Factories
{
    public static class ColorMapBuilder
    {
        public static Dictionary<EntityColor, Material> Build(List<EntityColorData> colorData)
        {
            var map = new Dictionary<EntityColor, Material>();
            foreach (var data in colorData)
                if (!map.ContainsKey(data.ColorType))
                    map.Add(data.ColorType, data.Material);
            return map;
        }
    }
}