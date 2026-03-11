using _Scripts.Runtime.Enums;
using UnityEngine;

namespace _Scripts.Runtime.Gameplay.Entities.Busses
{
    public class BusController : MonoBehaviour
    {
        [SerializeField] private Renderer busModelRenderer;

        private Transform _entrancePoint;
        private const int BussCapacity = 3;

        public EntityColor BusColor { get; private set; }
        public int ReservedCapacity { get; private set; }
        public int BoardedCapacity { get; private set; }


        public void Initialize(EntityColor color)
        {
            BusColor = color;
            ReservedCapacity = 0;
            BoardedCapacity = 0;
        }

        public void SetMaterial(Material material)
        {
            busModelRenderer.material = material;
        }

        public bool HasAvailableSlot => ReservedCapacity < BussCapacity;

        public bool OnPassengerBoarded()
        {
            BoardedCapacity++;
            return BoardedCapacity >= BussCapacity;
        }

        public Vector3 GetEntrancePosition()
        {
            if (ReservedCapacity >= BussCapacity) return Vector3.zero;
            ReservedCapacity++;
            return _entrancePoint != null ? _entrancePoint.position : transform.position;
        }
    }
}