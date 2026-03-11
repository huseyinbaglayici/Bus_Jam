using System.Collections.Generic;
using _Scripts.Runtime.Data.UnityObjects;
using _Scripts.Runtime.Data.ValueObjects;
using _Scripts.Runtime.Enums;
using _Scripts.Runtime.Factories;
using _Scripts.Runtime.Gameplay.Entities.Passenger;
using _Scripts.Runtime.Signals;
using _Scripts.Runtime.Systems;
using _Scripts.Runtime.Utils;
using UnityEngine;

namespace _Scripts.Runtime.Managers
{
    public class GridManager : MonoBehaviour
    {
        [Header("Prefabs")] [SerializeField] private GameObject walkableEmptyPrefab;
        [SerializeField] private GameObject obstructedPrefab;
        [SerializeField] private GameObject passengerPrefab;

        [Header("Entity Setup")] [SerializeField]
        private List<EntityColorData> colorDatabase;

        private Transform _gridHolder;
        private CellFactory _cellFactory;

        private Vector2Int GridSize { get; set; }
        private GridSystem LogicGrid { get; set; }

        private void Awake()
        {
            _cellFactory = new CellFactory(walkableEmptyPrefab, obstructedPrefab, passengerPrefab, colorDatabase);
        }

        private void OnEnable()
        {
            CoreGameSignals.Instance.OnLevelDataLoaded += GenerateLevel;
            ActiveLevelSignals.Instance.OnGetCenterOfActiveGrid += OnSendCenterOfActiveGrid;
            GridSignals.Instance.OnGetNode += GetNodeFromGrid;
            GridSignals.Instance.OnFreeNode += HandleFreeNode;
            GridSignals.Instance.OnCalculatePathToExit += HandleCalculatePathToExit;
        }

        private void OnDisable()
        {
            CoreGameSignals.Instance.OnLevelDataLoaded -= GenerateLevel;
            ActiveLevelSignals.Instance.OnGetCenterOfActiveGrid -= OnSendCenterOfActiveGrid;
            GridSignals.Instance.OnGetNode -= GetNodeFromGrid;
            GridSignals.Instance.OnFreeNode -= HandleFreeNode;
            GridSignals.Instance.OnCalculatePathToExit -= HandleCalculatePathToExit;
        }

        private void GenerateLevel(LevelDataSO levelData)
        {
            CreateHolder();
            LogicGrid = new GridSystem(levelData);
            GridSize = FetchGridParams(levelData);

            SpawnConcreteCells(levelData);
            CoreGameSignals.Instance.FireOnGridReady(levelData, GridSize.x, GridSize.y);
            var centerOfGrid = BusJamMathUtil.GetCenterOfGrid(GridSize.x, GridSize.y, ConstantUtil.SpaceModifier);
            CameraSignals.Instance.FireOnSetCameraZoom(GridSize.x, GridSize.y);
            CameraSignals.Instance.FireOnSetCameraPosition(centerOfGrid);
        }

        private Vector2Int FetchGridParams(LevelDataSO levelData)
        {
            return new Vector2Int(levelData.Cols, levelData.Rows);
        }

        private Vector3 OnSendCenterOfActiveGrid()
        {
            return BusJamMathUtil.GetCenterOfGrid(GridSize.x, GridSize.y, ConstantUtil.SpaceModifier);
        }

        private void SpawnConcreteCells(LevelDataSO levelData)
        {
            foreach (var cellData in levelData.GridCells)
            {
                Vector2Int gridPos = new Vector2Int(cellData.coordinates.x, cellData.coordinates.y);
                Vector3 worldPos = BusJamMathUtil.GridToWorldPosition(gridPos, ConstantUtil.SpaceModifier);

                GameObject spawnedCell =
                    _cellFactory.CreateCell(cellData, _gridHolder, worldPos);

                if (cellData.occupant == OccupantType.Passenger)
                {
                    if (spawnedCell.TryGetComponent(out PassengerController newPassenger))
                    {
                        PassengerSignals.Instance.FireOnRegisterPassenger(gridPos, newPassenger);
                    }
                }
            }
        }

        private void HandleFreeNode(int x, int y)
        {
            if (LogicGrid == null) return;
            LogicGrid.FreeNode(x, y);
        }

        private List<GridNode> HandleCalculatePathToExit(int x, int y)
        {
            if (LogicGrid == null) return null;
            return LogicGrid.CalculatePathToExit(x, y);
        }

        private GridNode GetNodeFromGrid(int x, int y)
        {
            if (LogicGrid != null) return LogicGrid.GetNode(x, y);
            return null;
        }

        private void CreateHolder()
        {
            _gridHolder = new GameObject("GridHolder").transform;
        }
    }
}