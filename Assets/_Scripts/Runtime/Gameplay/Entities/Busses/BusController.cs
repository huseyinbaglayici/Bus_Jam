using UnityEngine;

namespace _Scripts.Runtime.Gameplay.Entities.Busses
{
    public class BusController : MonoBehaviour
    {
        [SerializeField] private Renderer busModelRenderer;

        private const int BussCapacity = 3;

        public void SetMaterial(Material material)
        {
            busModelRenderer.material = material;
        }
    }
}