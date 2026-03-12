using UnityEngine;

namespace _Scripts.Runtime.Utils
{
    public static class BusJamMathUtil
    {
        public static Vector2Int WorldToGridPosition(Vector3 worldPosition, float spaceModifier = 1)
        {
            return new Vector2Int(
                Mathf.RoundToInt(worldPosition.x / spaceModifier),
                Mathf.RoundToInt(worldPosition.z / spaceModifier));
        }

        public static Vector3 GridToWorldPosition(Vector2Int gridPosition, float spaceModifier = 1)
        {
            return new Vector3(gridPosition.x * spaceModifier, 0, gridPosition.y * spaceModifier);
        }

        public static Vector3 GetCenterOfGrid(int width, int height, float spaceModifier = 1)
        {
            return new Vector3(
                (width - 1) / 2f * spaceModifier,
                0,
                (height - 1) / 2f * spaceModifier);
        }
    }
}