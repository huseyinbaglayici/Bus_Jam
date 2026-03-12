using System.Collections.Generic;
using _Scripts.Runtime.Data.UnityObjects;
using _Scripts.Runtime.Enums;
using _Scripts.Runtime.Extensions;
using _Scripts.Runtime.Factories;
using _Scripts.Runtime.Signals;
using _Scripts.Runtime.Utils;
using DG.Tweening;
using UnityEngine;

namespace _Scripts.Runtime.Gameplay.Entities.Busses
{
    public class BusManager : MonoBehaviour
    {
        [Header("Prefab")] [SerializeField] private GameObject busPrefab;
        private const string BusHolderName = "BusHolder";

        [Header("Color Database")] [SerializeField]
        private ColorDatabaseSO colorDatabase;

        private const float StationZOffset = 5.5f;
        private const float StationXOffset = 5f;
        private const float StationYOffset = 1.10f;
        private const float BusDepartDuration = 0.8f;
        private const float BusArriveDuration = 0.6f;
        private const float NextBusDelay = 0.65f;

        private readonly Vector3 _pivotCorrectionOffset = new Vector3(1.2f, 0, 0);

        private Transform _busHolder;
        private BusFactory _busFactory;
        private BusController _activeBus;
        private Queue<BusController> _busQueue = new Queue<BusController>();
        private Vector3 _stationPosition;
        private Vector3 _queueSpacing;


        private void Awake()
        {
            _busFactory = new BusFactory(busPrefab, colorDatabase.Colors);
            _queueSpacing = new Vector3(-StationXOffset, 0, 0);
        }

        private void OnEnable() => SubscribeEvents();

        private void SubscribeEvents()
        {
            CoreGameSignals.Instance.OnGridReady += OnGridReady;
            CoreGameSignals.Instance.OnPlay += HandleBusSeq;
            ActiveLevelSignals.Instance.OnGetBusCount += OnSendBusCount;
            BusSignals.Instance.OnGetBusPosition += HandleGetBusPosition;
            BusSignals.Instance.OnGetActiveBusColor += OnGetActiveBusColor;
            BusSignals.Instance.OnHasAvailableSlot += HandleHasAvailableSlot;
            BusSignals.Instance.OnPassengerBoardedBus += HandlePassengerBoardedBus;
            BusSignals.Instance.OnGetNextBusColor += HandleGetNextBusColor;
        }

        private void UnsubscribeEvents()
        {
            if (!ApplicationState.IsQuitting)
            {
                CoreGameSignals.Instance.OnGridReady -= OnGridReady;
                CoreGameSignals.Instance.OnPlay -= HandleBusSeq;
                ActiveLevelSignals.Instance.OnGetBusCount -= OnSendBusCount;
                BusSignals.Instance.OnGetBusPosition -= HandleGetBusPosition;
                BusSignals.Instance.OnGetActiveBusColor -= OnGetActiveBusColor;
                BusSignals.Instance.OnHasAvailableSlot -= HandleHasAvailableSlot;
                BusSignals.Instance.OnPassengerBoardedBus -= HandlePassengerBoardedBus;
                BusSignals.Instance.OnGetNextBusColor -= HandleGetNextBusColor;
            }
        }

        #region Initialization

        private void OnGridReady(LevelDataSO levelData, int gridXCount, int gridZCount)
        {
            CreateHolder();
            CalculateStationPosition(gridXCount, gridZCount);
            BusSignals.Instance.FireOnStationPositionReady(_stationPosition);
            SpawnConcreteBusses(levelData);
        }

        private void CalculateStationPosition(int gridXCount, int gridZCount)
        {
            float gridCenterX = ((gridZCount - 1) * ConstantUtil.SpaceModifier) / 2f;
            float gridTopZ = (gridXCount - 1) * ConstantUtil.SpaceModifier;
            _stationPosition = new Vector3(gridCenterX, StationYOffset, gridTopZ + StationZOffset)
                               + _pivotCorrectionOffset;
        }

        private void SpawnConcreteBusses(LevelDataSO levelData)
        {
            for (int i = levelData.BusSequence.Count - 1; i >= 0; i--)
            {
                int queueIndex = levelData.BusSequence.Count - 1 - i;
                var busData = levelData.BusSequence[i];
                Vector3 spawnPos = _stationPosition + _queueSpacing * (queueIndex + 1);

                GameObject spawnedBusObj = _busFactory.CreateBus(busData, _busHolder, spawnPos);
                if (!spawnedBusObj.TryGetComponent(out BusController busController)) continue;

                busController.Initialize(busData.color);
                _busQueue.Enqueue(busController);
            }
        }

        private void CreateHolder() => _busHolder = new GameObject(BusHolderName).transform;

        #endregion

        #region Bus Movement

        private void ActivateNextBus()
        {
            if (_busQueue.Count == 0)
            {
                _activeBus = null;
                return;
            }

            _activeBus = _busQueue.Dequeue();


            _activeBus.transform
                .DOMove(_stationPosition, BusArriveDuration)
                .SetEase(Ease.OutBack)
                .OnComplete(() => BusSignals.Instance.FireOnBusArrived(_activeBus.BusColor));

            ShiftQueueForward();
        }

        private void ShiftQueueForward()
        {
            int index = 1;
            foreach (var bus in _busQueue)
            {
                bus.transform
                    .DOMove(_stationPosition + _queueSpacing * index, 0.5f)
                    .SetEase(Ease.OutQuad);
                index++;
            }
        }

        private EntityColor HandleGetNextBusColor() =>
            _busQueue.Count > 0 ? _busQueue.Peek().BusColor : EntityColor.Default;


        private void SendActiveBusAway()
        {
            BusController departingBus = _activeBus;
            _activeBus = null;

            departingBus.transform
                .DOMoveX(departingBus.transform.position.x + 20f, BusDepartDuration)
                .SetEase(Ease.InBack)
                .OnComplete(() => Destroy(departingBus.gameObject));

            DOVirtual.DelayedCall(NextBusDelay, ActivateNextBus);
        }

        #endregion

        #region Signal Handlers

        private void HandleBusSeq()
        {
            if (_busQueue.Count > 0 && _activeBus == null) ActivateNextBus();
        }

        private Vector3 HandleGetBusPosition(EntityColor color) =>
            _activeBus != null ? _activeBus.GetEntrancePosition() : Vector3.zero;

        private int OnSendBusCount() => _busQueue.Count + (_activeBus != null ? 1 : 0);

        private EntityColor OnGetActiveBusColor() =>
            _activeBus != null ? _activeBus.BusColor : EntityColor.Default;

        private bool HandleHasAvailableSlot(EntityColor passengerColor) =>
            _activeBus != null && _activeBus.BusColor == passengerColor && _activeBus.HasAvailableSlot;

        private void HandlePassengerBoardedBus()
        {
            if (_activeBus == null) return;
            if (_activeBus.OnPassengerBoarded())
            {
                BusSignals.Instance.FireOnBusLeft(_activeBus.BusColor);
                SendActiveBusAway();
            }
        }

        #endregion

        private void OnDisable() => UnsubscribeEvents();
    }
}