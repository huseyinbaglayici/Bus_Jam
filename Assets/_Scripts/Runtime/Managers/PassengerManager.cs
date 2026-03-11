using System.Collections.Generic;
using _Scripts.Runtime.Data.ValueObjects;
using _Scripts.Runtime.Enums;
using _Scripts.Runtime.Gameplay.Entities.Passenger;
using _Scripts.Runtime.Signals;
using _Scripts.Runtime.Utils;
using UnityEngine;

namespace _Scripts.Runtime.Managers
{
    public class PassengerManager : MonoBehaviour
    {
        private readonly Dictionary<Vector2Int, PassengerController> _passengerRegistry = new();

        private void OnEnable() => SubscribeEvents();
        private void OnDisable() => UnSubscribeEvents();

        private void SubscribeEvents()
        {
            PassengerSignals.Instance.OnRegisterPassenger += RegisterPassenger;

            GridSignals.Instance.OnPassengerSelected += OnPassengerSelected;
        }

        private void UnSubscribeEvents()
        {
            PassengerSignals.Instance.OnRegisterPassenger -= RegisterPassenger;
            GridSignals.Instance.OnPassengerSelected -= OnPassengerSelected;
        }

        private void RegisterPassenger(Vector2Int gridPos, PassengerController controller)
        {
            _passengerRegistry[gridPos] = controller;
        }

        private void OnPassengerSelected(int x, int y)
        {
            Vector2Int gridPos = new Vector2Int(x, y);

            if (!_passengerRegistry.TryGetValue(gridPos, out var passengerController))
            {
                return;
            }

            PassengerEntity entity = passengerController.Entity;

            if (!TryGetAvailableTarget(passengerController, out Vector3 targetFinalPos,
                    out PassengerTargetType targetType))
            {
                return;
            }

            List<GridNode> gridPath = GridSignals.Instance.FireOnCalculatePathToExit(x, y);

            if (gridPath == null)
            {
                return;
            }

            Queue<Vector3> worldPath = ConvertGridPathToWorld(gridPath);
            worldPath.Enqueue(targetFinalPos);


            DispatchPathToPassenger(passengerController, entity, worldPath, targetType);
        }

        private bool TryGetAvailableTarget(PassengerController passengerController, out Vector3 targetPos,
            out PassengerTargetType targetType)
        {
            EntityColor passengerColor = passengerController.Entity.Color;
            targetPos = Vector3.zero;
            targetType = PassengerTargetType.None;

            if (BusSignals.Instance.FireOnHasAvailableSlot(passengerColor))
            {
                targetPos = BusSignals.Instance.FireOnGetBusPosition(passengerColor);
                targetType = PassengerTargetType.Bus;
                return true;
            }

            if (LineSignals.Instance.FireOnHasAvailableSlot())
            {
                targetPos = LineSignals.Instance.FireOnGetSlotPosition(passengerController);
                targetType = PassengerTargetType.Line;
                return true;
            }

            return false;
        }

        private Queue<Vector3> ConvertGridPathToWorld(List<GridNode> gridPath)
        {
            Queue<Vector3> pathQueue = new Queue<Vector3>();
            foreach (var node in gridPath)
            {
                Vector3 worldPos =
                    BusJamMathUtil.GridToWorldPosition(new Vector2Int(node.X, node.Y), ConstantUtil.SpaceModifier);
                pathQueue.Enqueue(worldPos);
            }

            return pathQueue;
        }

        private void DispatchPathToPassenger(PassengerController controller, PassengerEntity entity,
            Queue<Vector3> worldPath, PassengerTargetType targetType)
        {
            entity.CurrentTarget = targetType;
            entity.SetPath(new List<Vector3>(worldPath));

            GridSignals.Instance.FireOnFreeNode(entity.GridPosition.x, entity.GridPosition.y);
            _passengerRegistry.Remove(entity.GridPosition);
        }
    }
}