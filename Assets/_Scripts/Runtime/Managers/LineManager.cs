using System.Collections.Generic;
using System.Linq;
using _Scripts.Runtime.Data.UnityObjects;
using _Scripts.Runtime.Enums;
using _Scripts.Runtime.Factories;
using _Scripts.Runtime.Gameplay.Entities.Passenger;
using _Scripts.Runtime.Signals;
using _Scripts.Runtime.Utils;
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
        private const float LineZOffset = 2.9f;

        private readonly List<GameObject> _lineCells = new List<GameObject>();
        private readonly List<LineSlot> _lineSlots = new List<LineSlot>();
        private readonly HashSet<EntityColor> _incomingBusColors = new HashSet<EntityColor>();
        private Transform _lineHolder;
        private LineFactory _lineFactory;

        private void Awake() => _lineFactory = new LineFactory(lineCellPrefab);

        private void OnEnable()
        {
            CoreGameSignals.Instance.OnGridReady += GenerateLine;
            ActiveLevelSignals.Instance.OnGetPassengerLineCount += OnSendLineCount;
            LineSignals.Instance.OnHasAvailableSlot += HandleHasAvailableSlot;
            LineSignals.Instance.OnGetSlotPosition += HandleGetSlotPosition;
            BusSignals.Instance.OnBusArrived += CheckLineForMatchingPassengers;
            BusSignals.Instance.OnBusIncoming += HandleBusIncoming;
            BusSignals.Instance.OnBusLeft += HandleBusLeft;
        }

        #region Line Initialization

        private void GenerateLine(LevelDataSO levelData, int gridXCount, int gridZCount)
        {
            CreateHolder();

            float gridCenterX = ((gridXCount - 1) * ConstantUtil.SpaceModifier) / 2f;
            float gridTopZ = (gridZCount - 1) * ConstantUtil.SpaceModifier;

            _lineHolder.position = new Vector3(gridCenterX, 0, gridTopZ + LineZOffset);

            SpawnLineCells(levelData.PassengerLineCapacity);
        }

        private void SpawnLineCells(int capacity)
        {
            foreach (var cell in _lineCells) Destroy(cell);
            _lineCells.Clear();
            _lineSlots.Clear();

            float totalWidth = (capacity - 1) * ConstantUtil.SpaceModifier;
            float startX = -(totalWidth / 2f);

            for (int i = 0; i < capacity; i++)
            {
                Vector3 localPos = new Vector3(startX + i * ConstantUtil.SpaceModifier, 0, 0);

                GameObject spawnedCell = _lineFactory.CreateLineCell(_lineHolder, localPos);
                spawnedCell.transform.localPosition = localPos;

                _lineCells.Add(spawnedCell);
                _lineSlots.Add(new LineSlot(_lineHolder.TransformPoint(localPos)));
            }
        }

        private void CreateHolder() => _lineHolder = new GameObject("LineHolder").transform;

        #endregion

        #region Signal Handlers

        private void HandleBusIncoming(EntityColor color) => _incomingBusColors.Add(color);
        private void HandleBusLeft(EntityColor color) => _incomingBusColors.Remove(color);
        private int OnSendLineCount() => _lineCells.Count;
        private bool HandleHasAvailableSlot() => _lineSlots.Any(s => !s.IsOccupied);

        private void CheckLineForMatchingPassengers(EntityColor busColor)
        {
            foreach (var slot in _lineSlots)
            {
                if (!slot.IsOccupied || slot.Passenger.Entity.Color != busColor) continue;
                if (!BusSignals.Instance.FireOnHasAvailableSlot(busColor)) continue;

                slot.Passenger.RedirectToBus(BusSignals.Instance.FireOnGetBusPosition(busColor));
                slot.IsOccupied = false;
                slot.Passenger = null;
            }
        }

        private Vector3 HandleGetSlotPosition(PassengerController controller)
        {
            foreach (var slot in _lineSlots)
            {
                if (slot.Passenger != controller) continue;

                if (controller.Entity.CurrentTarget == PassengerTargetType.Bus)
                {
                    slot.IsOccupied = false;
                    slot.Passenger = null;
                    return Vector3.zero;
                }

                return slot.WorldPosition;
            }

            if (BusSignals.Instance.FireOnHasAvailableSlot(controller.Entity.Color))
            {
                controller.RedirectToBus(BusSignals.Instance.FireOnGetBusPosition(controller.Entity.Color));
                return Vector3.zero;
            }

            foreach (var slot in _lineSlots)
            {
                if (slot.IsOccupied) continue;

                slot.IsOccupied = true;
                slot.Passenger = controller;
                CheckGameOverCondition();
                return slot.WorldPosition;
            }

            return Vector3.zero;
        }

        private void CheckGameOverCondition()
        {
            if (!_lineSlots.All(s => s.IsOccupied)) return;
            if (_lineSlots.Any(s => _incomingBusColors.Contains(s.Passenger.Entity.Color))) return;

            CoreGameSignals.Instance.FireOnLevelFailed();
        }

        #endregion

        private void OnDisable()
        {
            CoreGameSignals.Instance.OnGridReady -= GenerateLine;
            ActiveLevelSignals.Instance.OnGetPassengerLineCount -= OnSendLineCount;
            LineSignals.Instance.OnHasAvailableSlot -= HandleHasAvailableSlot;
            LineSignals.Instance.OnGetSlotPosition -= HandleGetSlotPosition;
            BusSignals.Instance.OnBusArrived -= CheckLineForMatchingPassengers;
            BusSignals.Instance.OnBusIncoming -= HandleBusIncoming;
            BusSignals.Instance.OnBusLeft -= HandleBusLeft;
        }
    }
}