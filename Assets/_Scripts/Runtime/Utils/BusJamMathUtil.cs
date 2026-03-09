using Unity.Mathematics;
using UnityEngine;

namespace _Scripts.Runtime.Utils
{
    public static class BusJamMathUtil
    {
        public static Vector2Int WorldToGridPosition(Vector3 worldPosition, float spaceModifier = 1)
        {
            Vector2Int gridPosition = default;
            gridPosition.x = Mathf.RoundToInt(worldPosition.x / spaceModifier);
            gridPosition.y = Mathf.RoundToInt(worldPosition.z / spaceModifier);

            return gridPosition;
        }

        public static Vector3 GridToWorldPosition(Vector2Int gridPosition, float spaceModifier = 1)
        {
            return new Vector3(gridPosition.x, 0, gridPosition.y);
        }

        public static Vector3 GetCenterOfGrid(int widthParam, int heightParam, float spaceModifierParam = 1)
        {
            Vector3 centerOfGrid = GetGridCenter(widthParam, heightParam, spaceModifierParam);
            return centerOfGrid;
        }

        private static Vector3 GetGridCenter(int widthParam, int heightParam, float spaceModifierParam = 1)
        {
            float2 center = new float2
            {
                x = (widthParam - 1) / 2f,
                y = (heightParam - 1) / 2f
            };
            return new Vector3(center.x * spaceModifierParam, center.y * spaceModifierParam);
        }
    }
}