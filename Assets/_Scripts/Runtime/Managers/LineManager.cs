using System.Collections.Generic;
using System.Linq;
using _Scripts.Runtime.Data.UnityObjects;
using _Scripts.Runtime.Enums;
using _Scripts.Runtime.Extensions;
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
        private const float LineZOffset = 3f;
        private const string LineHolderName = "LineHolder";

        private readonly List<GameObject> _lineCells = new List<GameObject>();
        private readonly List<LineSlot> _lineSlots = new List<LineSlot>();
        private Transform _lineHolder;
        private LineFactory _lineFactory;
        private bool _isBusDeparting;


        private void Awake() => _lineFactory = new LineFactory(lineCellPrefab);

        private void OnEnable()
        {
            CoreGameSignals.Instance.OnGridReady += GenerateLine;
            LineSignals.Instance.OnHasAvailableSlot += HandleHasAvailableSlot;
            LineSignals.Instance.OnGetSlotPosition += HandleGetSlotPosition;
            BusSignals.Instance.OnBusArrived += CheckLineForMatchingPassengers;
            BusSignals.Instance.OnBusLeft += HandleBusLeft;
            BusSignals.Instance.OnBusArrived += HandleBusArrived;
        }

        #region Line Initialization

        private void GenerateLine(LevelDataSO levelData, int gridXCount, int gridZCount)
        {
            CreateHolder();
            float centerX = (gridZCount - 1) * ConstantUtil.SpaceModifier / 2f;
            float lineZ = (gridXCount - 1) * ConstantUtil.SpaceModifier + LineZOffset;

            _lineHolder.position = new Vector3(centerX, 0f, lineZ);
            SpawnLineCells(levelData.PassengerLineCapacity, centerX, lineZ);
        }

        private void SpawnLineCells(int capacity, float centerX, float lineZ)
        {
            foreach (var cell in _lineCells) Destroy(cell);
            _lineCells.Clear();
            _lineSlots.Clear();

            float totalWidth = (capacity - 1) * ConstantUtil.SpaceModifier;
            float startX = centerX - totalWidth / 2f;

            for (int i = 0; i < capacity; i++)
            {
                Vector3 worldPos = new Vector3(startX + i * ConstantUtil.SpaceModifier, 0f, lineZ);

                GameObject spawnedCell = _lineFactory.CreateLineCell(_lineHolder, worldPos);

                _lineCells.Add(spawnedCell);
                _lineSlots.Add(new LineSlot(worldPos));
            }
        }

        private void CreateHolder() => _lineHolder = new GameObject(LineHolderName).transform;

        #endregion

        #region Signal Handlers

        private bool HandleHasAvailableSlot() => _lineSlots.Any(s => !s.IsOccupied);
        private void HandleBusLeft(EntityColor color) => _isBusDeparting = true;
        private void HandleBusArrived(EntityColor color) => _isBusDeparting = false;

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

            if (_isBusDeparting)
            {
                EntityColor nextBusColor = BusSignals.Instance.FireOnGetNextBusColor();
                if (_lineSlots.Any(s => s.Passenger.Entity.Color == nextBusColor)) return;
            }

            CoreGameSignals.Instance.FireOnLevelFailed();
        }

        #endregion

        private void OnDisable()
        {
            if (!ApplicationState.IsQuitting)
            {
                CoreGameSignals.Instance.OnGridReady -= GenerateLine;
                LineSignals.Instance.OnHasAvailableSlot -= HandleHasAvailableSlot;
                LineSignals.Instance.OnGetSlotPosition -= HandleGetSlotPosition;
                BusSignals.Instance.OnBusArrived -= CheckLineForMatchingPassengers;
                BusSignals.Instance.OnBusLeft -= HandleBusLeft;
                BusSignals.Instance.OnBusArrived -= HandleBusArrived;
            }
        }
    }
}