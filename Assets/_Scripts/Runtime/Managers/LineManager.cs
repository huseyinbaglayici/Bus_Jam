using System.Collections.Generic;
using _Scripts.Runtime.Data.UnityObjects;
using _Scripts.Runtime.Factories;
using _Scripts.Runtime.Signals;
using UnityEngine;

namespace _Scripts.Runtime.Managers
{
    public class LineManager : MonoBehaviour
    {
        [SerializeField] private GameObject lineCellPrefab;
        private const float CellSpacing = 1.1f;
        private const float LineZOffset = 6f;
        private const float LineXAddition = 0f;


        private readonly List<GameObject> _lineCells = new List<GameObject>();
        private Transform _lineHolder;
        private LineFactory _lineFactory;

        private void Awake()
        {
            _lineFactory = new LineFactory(lineCellPrefab);
        }

        private void OnEnable()
        {
            CoreGameSignals.Instance.OnGridReady += GenerateLine;
        }

        private void OnDisable()
        {
            CoreGameSignals.Instance.OnGridReady -= GenerateLine;
        }

        private void GenerateLine(LevelDataSO levelData, Vector3 gridCenter)
        {
            CreateHolder();
            SpawnLineCells(levelData.PassengerLineCapacity, gridCenter);
        }

        private void SpawnLineCells(int capacity, Vector3 gridCenter)
        {
            float finalWidth = (capacity - 1) * CellSpacing;
            float startX = gridCenter.x - (finalWidth / 2f) + LineXAddition;
            for (int i = 0; i < capacity; i++)
            {
                float currentX = startX + (i * CellSpacing);
                Vector3 spawnPos = new Vector3(currentX, 0, gridCenter.z + LineZOffset);
                GameObject spawnedLineCell = _lineFactory.CreateLineCell(_lineHolder, spawnPos);
                _lineCells.Add(spawnedLineCell);
            }
        }

        private void CreateHolder()
        {
            _lineHolder = new GameObject("LineHolder").transform;
        }
    }
}