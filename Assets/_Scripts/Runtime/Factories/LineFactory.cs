using UnityEngine;

namespace _Scripts.Runtime.Factories
{
    public class LineFactory
    {
        private readonly GameObject _lineCellPrefab;

        public LineFactory(GameObject lineCellPrefab)
        {
            _lineCellPrefab = lineCellPrefab;
        }

        public GameObject CreateLineCell(Transform parent, Vector3 position)
        {
            return Object.Instantiate(_lineCellPrefab, position, Quaternion.identity, parent);
        }
    }
}