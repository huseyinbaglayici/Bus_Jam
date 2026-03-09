using System.Collections.Generic;
using _Scripts.Runtime.Data.UnityObjects;
using _Scripts.Runtime.Data.ValueObjects;
using _Scripts.Runtime.Factories;
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


        private const float SpaceModifer = 1.1f;

        private Transform _gridHolder;
        private CellFactory _cellFactory;

        public Vector2Int GridSize { get; private set; }

        public GridSystem LogicGrid { get; private set; }


        private void Awake()
        {
            _cellFactory = new CellFactory(walkableEmptyPrefab, obstructedPrefab, passengerPrefab, colorDatabase);
        }

        private void OnEnable()
        {
            CoreGameSignals.Instance.OnLevelDataLoaded += GenerateLevel;
            CameraSignals.Instance.FireOnSetCameraPosition(
                BusJamMathUtil.GetCenterOfGrid(GridSize.x, GridSize.y, SpaceModifer));
            ActiveLevelSignals.Instance.OnGetCenterOfActiveGrid += OnSendCenterOfActiveGrid;
        }

        private void OnDisable()
        {
            CoreGameSignals.Instance.OnLevelDataLoaded -= GenerateLevel;
            ActiveLevelSignals.Instance.OnGetCenterOfActiveGrid -= OnSendCenterOfActiveGrid;
        }

        private void GenerateLevel(LevelDataSO levelData)
        {
            CreateHolder();
            LogicGrid = new GridSystem(levelData);
            GridSize = FetchGridParams(levelData);

            Vector3 calculatedCenter = BusJamMathUtil.GetCenterOfGrid(levelData.Rows, levelData.Cols, SpaceModifer);
            CoreGameSignals.Instance.FireOnGridReady(levelData, calculatedCenter);
            SpawnConcreteCells(levelData);
        }

        private Vector2Int FetchGridParams(LevelDataSO levelData)
        {
            return new Vector2Int(levelData.Rows, levelData.Cols);
        }

        private Vector3 OnSendCenterOfActiveGrid()
        {
            return BusJamMathUtil.GetCenterOfGrid(GridSize.x, GridSize.y, SpaceModifer);
        }

        private void SpawnConcreteCells(LevelDataSO levelData)
        {
            foreach (var cellData in levelData.GridCells)
            {
                Vector3 worldPos =
                    BusJamMathUtil.GridToWorldPosition(new Vector2Int(cellData.coordinates.x, cellData.coordinates.y));
                _cellFactory.CreateCell(cellData, _gridHolder, worldPos, SpaceModifer);
            }
        }

        private void CreateHolder()
        {
            _gridHolder = new GameObject("GridHolder").transform;
        }
    }
}