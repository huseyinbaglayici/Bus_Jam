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
            _colorMap = ColorMapBuilder.Build(colorData);
        }

        public GameObject CreateCell(CellSaveData cellData, Transform parent, Vector3 position)
        {
            var spawnedCell = Object.Instantiate(DeterminePrefab(cellData), position, Quaternion.identity, parent);

            if (cellData.occupant == OccupantType.Passenger)
                ConfigurePassenger(spawnedCell, cellData);

            return spawnedCell;
        }

        private GameObject DeterminePrefab(CellSaveData cellData)
        {
            if (cellData.occupant == OccupantType.Passenger) return _passengerPrefab;
            if (cellData.type == CellType.Obstructed) return _obstructedPrefab;
            return _walkableEmptyPrefab;
        }

        private void ConfigurePassenger(GameObject entity, CellSaveData cellData)
        {
            if (!_colorMap.TryGetValue(cellData.color, out var material))
            {
                Debug.LogWarning($"Material not found for color: {cellData.color}");
                return;
            }

            if (entity.TryGetComponent(out PassengerController controller))
                controller.Initialize(material, cellData.color,
                    new Vector2Int(cellData.coordinates.x, cellData.coordinates.y));
        }
    }
}