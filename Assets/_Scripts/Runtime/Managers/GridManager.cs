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

        private const string GridHolderName = "GridHolder";

        [Header("Color Database")] [SerializeField]
        private ColorDatabaseSO colorDatabase;

        private Transform _gridHolder;
        private CellFactory _cellFactory;
        private GridSystem _logicGrid;
        private Vector2Int _gridSize;

        private void Awake()
        {
            _cellFactory = new CellFactory(walkableEmptyPrefab, obstructedPrefab, passengerPrefab,
                colorDatabase.Colors);
        }

        private void OnEnable()
        {
            CoreGameSignals.Instance.OnLevelDataLoaded += GenerateLevel;
            GridSignals.Instance.OnGetNode += GetNodeFromGrid;
            GridSignals.Instance.OnFreeNode += HandleFreeNode;
            GridSignals.Instance.OnCalculatePathToExit += HandleCalculatePathToExit;
        }

        #region Initialization

        private void GenerateLevel(LevelDataSO levelData)
        {
            CreateHolder();

            _logicGrid = new GridSystem(levelData);
            _gridSize = new Vector2Int(levelData.Cols, levelData.Rows);

            SpawnConcreteCells(levelData);

            var centerOfGrid = BusJamMathUtil.GetCenterOfGrid(_gridSize.x, _gridSize.y, ConstantUtil.SpaceModifier);
            CoreGameSignals.Instance.FireOnGridReady(levelData, _gridSize.x, _gridSize.y);
            CameraSignals.Instance.FireOnSetCameraZoom(_gridSize.x, _gridSize.y);
            CameraSignals.Instance.FireOnSetCameraPosition(centerOfGrid);
        }

        private void SpawnConcreteCells(LevelDataSO levelData)
        {
            foreach (var cellData in levelData.GridCells)
            {
                Vector2Int gridPos = new Vector2Int(cellData.coordinates.x, cellData.coordinates.y);
                Vector3 worldPos = BusJamMathUtil.GridToWorldPosition(gridPos, ConstantUtil.SpaceModifier);

                GameObject spawnedCell = _cellFactory.CreateCell(cellData, _gridHolder, worldPos);

                if (cellData.occupant == OccupantType.Passenger &&
                    spawnedCell.TryGetComponent(out PassengerController passenger))
                {
                    PassengerSignals.Instance.FireOnRegisterPassenger(gridPos, passenger);
                }
            }
        }

        private void CreateHolder() => _gridHolder = new GameObject(GridHolderName).transform;

        #endregion

        #region Signal Handlers

        private Vector3 OnSendCenterOfActiveGrid() =>
            BusJamMathUtil.GetCenterOfGrid(_gridSize.x, _gridSize.y, ConstantUtil.SpaceModifier);

        private GridNode GetNodeFromGrid(int x, int y) => _logicGrid?.GetNode(x, y);

        private void HandleFreeNode(int x, int y) => _logicGrid?.FreeNode(x, y);

        private List<GridNode> HandleCalculatePathToExit(int x, int y) =>
            _logicGrid?.CalculatePathToExit(x, y);

        #endregion

        private void OnDisable()
        {
            if (!CoreGameSignals.IsAvailable) return;
            CoreGameSignals.Instance.OnLevelDataLoaded -= GenerateLevel;
            GridSignals.Instance.OnGetNode -= GetNodeFromGrid;
            GridSignals.Instance.OnFreeNode -= HandleFreeNode;
            GridSignals.Instance.OnCalculatePathToExit -= HandleCalculatePathToExit;
        }
    }
}