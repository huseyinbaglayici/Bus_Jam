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
            _colorMap = new Dictionary<EntityColor, Material>();
            foreach (var data in colorData)
            {
                if (!_colorMap.ContainsKey(data.ColorType))
                {
                    _colorMap.Add(data.ColorType, data.Material);
                }
            }
        }

        public GameObject CreateBus(BusLineSaveData busData, Transform parent, Vector3 position)
        {
            GameObject spawnedBus = Object.Instantiate(_busPrefab, position, Quaternion.identity, parent);

            ApplyColor(spawnedBus, busData.color);
            return spawnedBus;
        }

        private void ApplyColor(GameObject spawnedEntity, EntityColor colorEnum)
        {
            if (_colorMap.TryGetValue(colorEnum, out var material))
            {
                if (spawnedEntity.TryGetComponent(out BusController controller))
                {
                    controller.SetMaterial(material);
                }
            }
            else
            {
                Debug.LogWarning("cannotfoundmat");
            }
        }
    }
}