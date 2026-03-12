using _Scripts.Runtime.Data.ValueObjects;
using _Scripts.Runtime.Enums;
using _Scripts.Runtime.Signals;
using _Scripts.Runtime.Utils;
using UnityEngine;

namespace _Scripts.Runtime.Managers
{
    public class SelectionManager : MonoBehaviour
    {
        private void OnEnable() => InputSignals.Instance.OnInputTaken += HandleInput;

        private void HandleInput(Vector3 worldPoint)
        {
            Vector2Int gridPos = BusJamMathUtil.WorldToGridPosition(worldPoint, ConstantUtil.SpaceModifier);

            GridNode node = GridSignals.Instance.FireOnGetNode(gridPos.x, gridPos.y);
            if (node == null || node.Occupant != OccupantType.Passenger) return;

            GridSignals.Instance.FireOnPassengerSelected(node.X, node.Y);
        }

        private void OnDisable()
        {
            if (!CoreGameSignals.IsAvailable) return;
            InputSignals.Instance.OnInputTaken -= HandleInput;
        }
    }
}