using System.Collections.Generic;
using _Scripts.Runtime.Data.ValueObjects;
using _Scripts.Runtime.Enums;
using _Scripts.Runtime.Gameplay.Entities.Busses;
using UnityEngine;

namespace _Scripts.Runtime.Factories
{
    public class BusFactory
    {
        private readonly GameObject _busPrefab;
        private readonly Dictionary<EntityColor, Material> _colorMap;

        public BusFactory(GameObject busPrefab, List<EntityColorData> colorData)
        {
            _busPrefab = busPrefab;
            _colorMap = ColorMapBuilder.Build(colorData);
        }

        public GameObject CreateBus(BusLineSaveData busData, Transform parent, Vector3 position)
        {
            var spawnedBus = Object.Instantiate(_busPrefab, position, Quaternion.identity, parent);
            ApplyColor(spawnedBus, busData.color);
            return spawnedBus;
        }

        private void ApplyColor(GameObject entity, EntityColor color)
        {
            if (!_colorMap.TryGetValue(color, out var material))
            {
                Debug.LogWarning($"Material not found for color: {color}");
                return;
            }

            if (entity.TryGetComponent(out BusController controller))
                controller.SetMaterial(material);
        }
    }
}