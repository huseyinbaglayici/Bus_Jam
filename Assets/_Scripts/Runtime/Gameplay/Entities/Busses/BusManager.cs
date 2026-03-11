using System.Collections.Generic;
using _Scripts.Runtime.Data.UnityObjects;
using _Scripts.Runtime.Data.ValueObjects;
using _Scripts.Runtime.Enums;
using _Scripts.Runtime.Factories;
using _Scripts.Runtime.Signals;
using DG.Tweening;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace _Scripts.Runtime.Gameplay.Entities.Busses
{
    public class BusManager : MonoBehaviour
    {
        [Header("Prefab")] [SerializeField] private GameObject busPrefab;

        [SerializeField] private List<EntityColorData> colorDatabase;

        private const float StationZOffset = 5.5f;
        private const float StationXOffset = 5f;
        private const float StationYOffset = 1.10f;


        private Transform _busHolder;
        private BusFactory _busFactory;
        private Queue<BusController> _busQueue = new Queue<BusController>();
        private BusController _activeBus;
        private readonly Vector3 _pivotCorrectionOffset = new Vector3(1.2f, 0, 0);
        private const float XMultiplier = 1.3f;
        private Vector3 _activeBusPosition;
        private Vector3 _queueSpacing;

        private void Awake()
        {
            _busFactory = new BusFactory(busPrefab, colorDatabase);
            _queueSpacing = new Vector3(-StationXOffset, 0, 0);
        }

        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            CoreGameSignals.Instance.OnGridReady += GenerateBusses;
            ActiveLevelSignals.Instance.OnGetBusCount += OnSendBusCount;
            BusSignals.Instance.OnGetActiveBusColor += OnGetActiveBusColor;
            BusSignals.Instance.OnHasAvailableSlot += HandleHasAvailableSlot;
            BusSignals.Instance.OnGetSlotPosition += HandleGetSlotPosition;
            BusSignals.Instance.OnPassengerBoardedBus += HandlePassengerBoardedBus;
            CoreGameSignals.Instance.OnPlay += HandleBusSeq;
        }

        private void GenerateBusses(LevelDataSO levelData, int gridXCount, int gridZCount)
        {
            CreateHolder();
            float gridCenterX = ((gridXCount - 1) * XMultiplier) / 2f;
            float gridTopZ = (gridZCount - 1) * 1.1f;
            float finalBusZ = gridTopZ + StationZOffset;
            _activeBusPosition = new Vector3(gridCenterX, StationYOffset, finalBusZ) + _pivotCorrectionOffset;

            SpawnConcreteBusses(levelData);
        }


        private void HandleBusSeq()
        {
            if (_busQueue.Count > 0 && _activeBus == null)
                ActivateNextBus();
        }

        private void SpawnConcreteBusses(LevelDataSO levelData)
        {
            int queueIndex = 0;
            for (int i = levelData.BusSequence.Count - 1; i >= 0; i--)
            {
                var busData = levelData.BusSequence[i];
                Vector3 spawnPos = _activeBusPosition + (_queueSpacing * (queueIndex + 1));
                GameObject spawnedBusObj = _busFactory.CreateBus(busData, _busHolder, spawnPos);

                if (spawnedBusObj.TryGetComponent(out BusController busController))
                {
                    busController.Initialize(busData.color);
                    _busQueue.Enqueue(busController);
                }

                queueIndex++;
            }
        }

        #region BusMovement Logic

        private void ActivateNextBus()
        {
            if (_busQueue.Count == 0)
            {
                _activeBus = null;
                return;
            }

            _activeBus = _busQueue.Dequeue();
            _activeBus.transform.DOMove(_activeBusPosition, 0.6f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                BusSignals.Instance.FireOnBusArrived(_activeBus.BusColor);
            });

            ShiftWaitingBussesForward();
        }

        private void ShiftWaitingBussesForward()
        {
            int index = 1;
            foreach (var bus in _busQueue)
            {
                Vector3 targetWaitPos = _activeBusPosition + (_queueSpacing * index);
                bus.transform.DOMove(targetWaitPos, 0.5f).SetEase(Ease.OutQuad);
                index++;
            }
        }

        private void SendActiveBusAway()
        {
            BusController departingBus = _activeBus;

            departingBus.transform.DOMoveX(departingBus.transform.position.x + 20f, 0.8f)
                .SetEase(Ease.InBack)
                .OnComplete(() => { Destroy(departingBus.gameObject); });
            DOVirtual.DelayedCall(0.65f, ActivateNextBus);
        }

        #endregion


        #region Signal Handlers

        private int OnSendBusCount() => _busQueue.Count + (_activeBus != null ? 1 : 0);

        private EntityColor OnGetActiveBusColor()
        {
            return _activeBus != null ? _activeBus.BusColor : EntityColor.Default;
        }

        private bool HandleHasAvailableSlot(EntityColor passengerColor)
        {
            if (_activeBus == null) return false;
            return _activeBus.BusColor == passengerColor && _activeBus.HasAvailableSlot;
        }

        private Vector3 HandleGetSlotPosition(EntityColor passengerColor)
        {
            if (_activeBus == null) return Vector3.zero;
            return _activeBus.GetEntrancePosition();
        }

        private void HandlePassengerBoardedBus()
        {
            if (_activeBus == null) return;

            bool isBusFull = _activeBus.OnPassengerBoarded();

            if (isBusFull)
            {
                SendActiveBusAway();
            }
        }

        #endregion

        private void OnDisable()
        {
            UnsubscribeEvents();
        }

        private void UnsubscribeEvents()
        {
            CoreGameSignals.Instance.OnGridReady -= GenerateBusses;
            ActiveLevelSignals.Instance.OnGetBusCount -= OnSendBusCount;
            BusSignals.Instance.OnGetActiveBusColor -= OnGetActiveBusColor;
            BusSignals.Instance.OnHasAvailableSlot -= HandleHasAvailableSlot;
            BusSignals.Instance.OnGetSlotPosition -= HandleGetSlotPosition;
            BusSignals.Instance.OnPassengerBoardedBus -= HandlePassengerBoardedBus;
            CoreGameSignals.Instance.OnPlay -= HandleBusSeq;
        }

        private void CreateHolder()
        {
            _busHolder = new GameObject("BusHolder").transform;
        }
    }
}