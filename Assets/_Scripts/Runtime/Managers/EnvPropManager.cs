using _Scripts.Runtime.Signals;
using UnityEngine;

namespace _Scripts.Runtime.Managers
{
    public class EnvPropManager : MonoBehaviour
    {
        [SerializeField] private Transform road;
        private const float RoadZOffset = -2f;

        private void OnEnable()
        {
            BusSignals.Instance.OnStationPositionReady += OnAlignRoad;
        }

        private void OnAlignRoad(Vector3 stationPosition)
        {
            road.position = new Vector3(
                stationPosition.x,
                road.position.y,
                stationPosition.z - RoadZOffset // bus pozisyonundan biraz geri
            );
        }

        private void OnDisable()
        {
            BusSignals.Instance.OnStationPositionReady -= OnAlignRoad;
        }
    }
}