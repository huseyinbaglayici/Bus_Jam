using _Scripts.Runtime.Enums;
using UnityEngine;

namespace _Scripts.Runtime.Gameplay.Entities.Busses
{
    public class BusController : MonoBehaviour
    {
        [SerializeField] private Renderer busModelRenderer;
        [SerializeField] private Transform entrancePoint;
        private const int BusCapacity = 3;

        public EntityColor BusColor { get; private set; }
        public int ReservedCapacity { get; private set; }
        public int BoardedCapacity { get; private set; }
        public bool HasAvailableSlot => ReservedCapacity < BusCapacity;

        public void Initialize(EntityColor color) => BusColor = color;

        public void SetMaterial(Material material) => busModelRenderer.material = material;

        public bool OnPassengerBoarded()
        {
            BoardedCapacity++;
            return BoardedCapacity >= BusCapacity;
        }

        public Vector3 GetEntrancePosition()
        {
            if (ReservedCapacity >= BusCapacity) return Vector3.zero;
            ReservedCapacity++;
            return entrancePoint != null ? entrancePoint.position : transform.position;
        }
    }
}