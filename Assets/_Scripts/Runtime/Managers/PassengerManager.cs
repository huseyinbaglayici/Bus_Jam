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

        private void OnEnable()
        {
            PassengerSignals.Instance.OnRegisterPassenger += RegisterPassenger;
            GridSignals.Instance.OnPassengerSelected += OnPassengerSelected;
        }

        private void OnPassengerSelected(int x, int y)
        {
            Vector2Int gridPos = new Vector2Int(x, y);
            if (!_passengerRegistry.TryGetValue(gridPos, out var controller)) return;
            if (controller.Entity.CurrentTarget != PassengerTargetType.None) return;

            List<GridNode> gridPath = GridSignals.Instance.FireOnCalculatePathToExit(x, y);
            if (gridPath == null)
            {
                controller.Entity.PathPoints.Clear();
                controller.Entity.IsTapped = true;
                controller.Entity.IsMoveable = false;
                return;
            }

            controller.Entity.IsTapped = false;

            if (!TryGetAvailableTarget(controller, out Vector3 targetPos, out PassengerTargetType targetType)) return;
            Queue<Vector3> worldPath = ConvertGridPathToWorld(gridPath);
            worldPath.Enqueue(targetPos);
            DispatchPathToPassenger(controller, worldPath, targetType);
        }

        private bool TryGetAvailableTarget(PassengerController controller, out Vector3 targetPos,
            out PassengerTargetType targetType)
        {
            EntityColor color = controller.Entity.Color;
            targetPos = Vector3.zero;
            targetType = PassengerTargetType.None;

            if (BusSignals.Instance.FireOnHasAvailableSlot(color))
            {
                targetPos = BusSignals.Instance.FireOnGetBusPosition(color);
                targetType = PassengerTargetType.Bus;
                return true;
            }

            if (LineSignals.Instance.FireOnHasAvailableSlot())
            {
                targetPos = LineSignals.Instance.FireOnGetSlotPosition(controller);
                targetType = PassengerTargetType.Line;
                return true;
            }

            return false;
        }

        private Queue<Vector3> ConvertGridPathToWorld(List<GridNode> gridPath)
        {
            var pathQueue = new Queue<Vector3>();
            foreach (var node in gridPath)
                pathQueue.Enqueue(BusJamMathUtil.GridToWorldPosition(
                    new Vector2Int(node.X, node.Y), ConstantUtil.SpaceModifier));
            return pathQueue;
        }

        private void DispatchPathToPassenger(PassengerController controller, Queue<Vector3> worldPath,
            PassengerTargetType targetType)
        {
            PassengerEntity entity = controller.Entity;
            entity.CurrentTarget = targetType;
            entity.SetPath(new List<Vector3>(worldPath));

            GridSignals.Instance.FireOnFreeNode(entity.GridPosition.x, entity.GridPosition.y);
            _passengerRegistry.Remove(entity.GridPosition);
        }

        private void RegisterPassenger(Vector2Int gridPos, PassengerController controller) =>
            _passengerRegistry[gridPos] = controller;

        private void OnDisable()
        {
            PassengerSignals.Instance.OnRegisterPassenger -= RegisterPassenger;
            GridSignals.Instance.OnPassengerSelected -= OnPassengerSelected;
        }
    }
}