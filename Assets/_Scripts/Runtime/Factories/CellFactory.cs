using System.Collections.Generic;
using _Scripts.Runtime.Data.ValueObjects;
using _Scripts.Runtime.Enums;
using _Scripts.Runtime.Gameplay.Entities.Passenger;
using UnityEngine;

namespace _Scripts.Runtime.Factories
{
    public class CellFactory
    {
        private readonly GameObject _walkableEmptyPrefab;
        private readonly GameObject _obstructedPrefab;
        private readonly GameObject _passengerPrefab;
        private readonly Dictionary<EntityColor, Material> _colorMap;

        public CellFactory(GameObject walkable, GameObject obstructed, GameObject passenger,
            List<EntityColorData> colorData)
        {
            _walkableEmptyPrefab = walkable;
            _obstructedPrefab = obstructed;
            _passengerPrefab = passenger;
            _colorMap = new Dictionary<EntityColor, Material>();
            foreach (var data in colorData)
            {
                if (!_colorMap.ContainsKey(data.ColorType))
                    _colorMap.Add(data.ColorType, data.Material);
            }
        }

        public void CreateCell(CellSaveData cellData, Transform parent, Vector3 position, float spaceModifier = 1)
        {
            GameObject prefabToSpawn = DeterminePrefab(cellData);
            GameObject spawnedCell = Object.Instantiate(prefabToSpawn,
                new Vector3(position.x * spaceModifier, position.y, position.z * spaceModifier), Quaternion.identity,
                parent);

            if (cellData.occupant == OccupantType.Passenger)
            {
                ApplyColor(spawnedCell, cellData.color);
            }
        }

        private GameObject DeterminePrefab(CellSaveData cellData)
        {
            if (cellData.occupant == OccupantType.Passenger)
                return _passengerPrefab;
            if (cellData.type == CellType.Obstructed)
                return _obstructedPrefab;

            return _walkableEmptyPrefab;
        }


        private void ApplyColor(GameObject spawnedEntity, EntityColor colorEnum)
        {
            if (_colorMap.TryGetValue(colorEnum, out var material))
            {
                if (spawnedEntity.TryGetComponent(out PassengerController controller))
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