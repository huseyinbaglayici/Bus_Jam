using _Scripts.Runtime.Data.ValueObjects;
using _Scripts.Runtime.Enums;
using _Scripts.Runtime.Signals;
using _Scripts.Runtime.Utils;
using UnityEngine;

namespace _Scripts.Runtime.Managers
{
    public class SelectionManager : MonoBehaviour
    {
        private void OnEnable()
        {
            InputSignals.Instance.OnInputTaken += HandleInput;
        }

        private void HandleInput(Vector3 worldPoint)
        {
            Vector2Int gridPos = BusJamMathUtil.WorldToGridPosition(worldPoint, ConstantUtil.SpaceModifier);

            var capturedNode = GridSignals.Instance.FireOnGetNode(gridPos.x, gridPos.y);
            if (capturedNode == null) return;
            if (!IsNodeSelectable(capturedNode)) return;
            DispatchSelectedNode(new Vector2Int(capturedNode.X, capturedNode.Y));
        }

        private bool IsNodeSelectable(GridNode capturedNode)
        {
            return capturedNode.Occupant == OccupantType.Passenger;
        }

        private void DispatchSelectedNode(Vector2Int entityPos)
        {
            GridSignals.Instance.FireOnPassengerSelected(entityPos.x, entityPos.y);
        }

        private void OnDisable()
        {
            InputSignals.Instance.OnInputTaken -= HandleInput;
        }
    }
}