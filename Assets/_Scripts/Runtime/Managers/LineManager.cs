using System.Collections.Generic;
using _Scripts.Runtime.Data.UnityObjects;
using _Scripts.Runtime.Enums;
using _Scripts.Runtime.Factories;
using _Scripts.Runtime.Gameplay.Entities.Passenger;
using _Scripts.Runtime.Signals;
using UnityEngine;

namespace _Scripts.Runtime.Managers
{
    public class LineSlot
    {
        public Vector3 WorldPosition;
        public bool IsOccupied;
        public PassengerController Passenger;

        public LineSlot(Vector3 position)
        {
            WorldPosition = position;
            IsOccupied = false;
            Passenger = null;
        }
    }

    public class LineManager : MonoBehaviour
    {
        [SerializeField] private GameObject lineCellPrefab;
        private const float CellSpacing = 1.1f;
        private const float LineZOffset = 1f;
        private const float XMultiplier = 2.5f;

        private readonly List<GameObject> _lineCells = new List<GameObject>();
        private readonly List<LineSlot> _lineSlots = new List<LineSlot>();
        private Transform _lineHolder;
        private LineFactory _lineFactory;
        private int _lineLimit;

        private void Awake()
        {
            _lineFactory = new LineFactory(lineCellPrefab);
        }

        private void OnEnable()
        {
            CoreGameSignals.Instance.OnGridReady += GenerateLine;
            ActiveLevelSignals.Instance.OnGetPassengerLineCount += OnSendLineCount;
            LineSignals.Instance.OnHasAvailableSlot += HandleHasAvailableSlot;
            LineSignals.Instance.OnGetSlotPosition += HandleGetSlotPosition;
            BusSignals.Instance.OnBusArrived += CheckLineForMatchingPassengers;
        }

        private void CheckLineForMatchingPassengers(EntityColor busColor)
        {
            foreach (var slot in _lineSlots)
            {
                if (!slot.IsOccupied) continue;

                if (slot.Passenger.Entity.Color == busColor)
                {
                    if (BusSignals.Instance.FireOnHasAvailableSlot(busColor))
                    {
                        Vector3 busPos = BusSignals.Instance.FireOnGetBusPosition(busColor);
                        slot.Passenger.RedirectToBus(busPos);

                        slot.IsOccupied = false;
                        slot.Passenger = null;
                    }
                }
            }
        }


        private bool HandleHasAvailableSlot()
        {
            foreach (var slot in _lineSlots)
            {
                if (!slot.IsOccupied) return true;
            }

            return false;
        }

        private Vector3 HandleGetSlotPosition(PassengerController controller)
        {
            foreach (var slot in _lineSlots)
            {
                if (!slot.IsOccupied)
                {
                    slot.IsOccupied = true;
                    slot.Passenger = controller;
                    CheckGameOverCondition();
                    return slot.WorldPosition;
                }
            }

            return Vector3.zero;
        }

        private void CheckGameOverCondition()
        {
            bool isFull = true;
            foreach (var slot in _lineSlots)
            {
                if (!slot.IsOccupied)
                {
                    isFull = false;
                    break;
                }
            }

            if (isFull)
            {
                CoreGameSignals.Instance.FireOnLevelFailed();
            }
        }

        private int OnSendLineCount()
        {
            return _lineCells.Count;
        }

        #region Line Initialization

        private void GenerateLine(LevelDataSO levelData, int gridXCount, int gridZCount)
        {
            CreateHolder();
            _lineLimit = levelData.PassengerLineCapacity;
            float gridCenterX = ((gridXCount - 1) * XMultiplier) / 2f;
            float gridTopZ = (gridZCount - 1) * 1.1f;
            float finalZPosition = gridTopZ + LineZOffset;
            _lineHolder.position = new Vector3(gridCenterX, 0, finalZPosition);
            SpawnLineCells(levelData.PassengerLineCapacity);
        }

        private void SpawnLineCells(int capacity)
        {
            foreach (var cell in _lineCells) Destroy(cell);
            _lineCells.Clear();
            _lineSlots.Clear();

            float totalWidth = (capacity - 1) * CellSpacing;
            float startX = -(totalWidth / 2f);

            for (int i = 0; i < capacity; i++)
            {
                float localX = startX + (i * CellSpacing);
                Vector3 localPos = new Vector3(localX, 0, 0);

                GameObject spawnedLineCell = _lineFactory.CreateLineCell(_lineHolder, localPos);
                spawnedLineCell.transform.localPosition = localPos;

                _lineCells.Add(spawnedLineCell);
        
                _lineSlots.Add(new LineSlot(spawnedLineCell.transform.position));
            }
        }

        private void CreateHolder()
        {
            _lineHolder = new GameObject("LineHolder").transform;
        }

        #endregion

        private void OnDisable()
        {
            CoreGameSignals.Instance.OnGridReady -= GenerateLine;
            ActiveLevelSignals.Instance.OnGetPassengerLineCount -= OnSendLineCount;
            LineSignals.Instance.OnHasAvailableSlot -= HandleHasAvailableSlot;
            LineSignals.Instance.OnGetSlotPosition -= HandleGetSlotPosition;
            BusSignals.Instance.OnBusArrived -= CheckLineForMatchingPassengers;
        }
    }
}