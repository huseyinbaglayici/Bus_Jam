using System;
using System.Collections.Generic;
using _Scripts.Runtime.Data.UnityObjects;
using _Scripts.Runtime.Data.ValueObjects;
using _Scripts.Runtime.Factories;
using _Scripts.Runtime.Signals;
using UnityEngine;

namespace _Scripts.Runtime.Gameplay.Entities.Busses
{
    public class BusManager : MonoBehaviour
    {
        [Header("Prefab")] [SerializeField] private GameObject busPrefab;
        [SerializeField] private Vector3 initialSpawnPos;

        [Header("Entity Setup")] [SerializeField]
        private List<EntityColorData> colorDatabase;

        private const float StationZOffset = 9f;
        private const float StationXOffset = 5f;
        private const float StationYOffset = 1.10f;


        private Transform _busHolder;
        private BusFactory _busFactory;

        private void Awake()
        {
            _busFactory = new BusFactory(busPrefab, colorDatabase);
        }

        private void OnEnable()
        {
            CoreGameSignals.Instance.OnGridReady += GenerateBusses;
        }

        private void GenerateBusses(LevelDataSO levelData, Vector3 calculatedCenter)
        {
            CreateHolder();
            SpawnConcreteBusses(levelData, calculatedCenter);
        }

        private void OnDisable()
        {
            CoreGameSignals.Instance.OnGridReady -= GenerateBusses;
        }

        private void SpawnConcreteBusses(LevelDataSO levelData, Vector3 gridCenter)
        {
            Vector3 stationPos = new Vector3(
                gridCenter.x,
                StationYOffset,
                gridCenter.z + StationZOffset
            );

            Vector3 queueSpacing = new Vector3(-StationXOffset, 0f, 0f);
            for (int i = 0; i < levelData.BusSequence.Count; i++)
            {
                var busData = levelData.BusSequence[i];
                Vector3 spawnPos = stationPos + (queueSpacing * i);

                _busFactory.CreateBus(busData, _busHolder, spawnPos);
            }
        }

        private void CreateHolder()
        {
            _busHolder = new GameObject("BusHolder").transform;
        }
    }
}