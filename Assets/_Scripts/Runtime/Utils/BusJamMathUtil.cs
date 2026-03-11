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
            return new Vector3(
                gridPosition.x * spaceModifier,
                0,
                gridPosition.y * spaceModifier
            );
        }

        public static Vector3 GetCenterOfGrid(int widthParam, int heightParam, float spaceModifierParam = 1)
        {
            return GetGridCenter(widthParam, heightParam, spaceModifierParam);
        }

        private static Vector3 GetGridCenter(int widthParam, int heightParam, float spaceModifierParam = 1)
        {
            float2 center = new float2
            {
                x = (widthParam - 1) / 2f,
                y = (heightParam - 1) / 2f
            };
            Debug.Log($"Calculated Grid Center (Grid Coordinates): {center}");
            return GridToWorldPosition(new Vector2Int((int)center.x, (int)center.y), spaceModifierParam);
        }

        public static Vector3 CalculateQueuePosition(Vector3 activeBusPos, Vector3 queueSpacing, int queueIndex)
        {
            return activeBusPos + (queueSpacing * (queueIndex + 1));
        }

        public static float GetGridTopEdgeZ(Vector3 gridCenter, int heightParam, float spaceModifierParam = 1f)
        {
            float halfHeight = (heightParam * spaceModifierParam) / 2f;
            return gridCenter.z + halfHeight;
        }
    }
}