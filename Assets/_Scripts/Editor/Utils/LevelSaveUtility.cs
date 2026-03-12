using System.Collections.Generic;
using System.IO;
using _Scripts.Runtime.Data.UnityObjects;
using _Scripts.Runtime.Data.ValueObjects;
using UnityEditor;
using UnityEngine;

namespace _Scripts.Editor.Utils
{
    public static class LevelSaveUtility
    {
        public static void SaveLevel(CellSaveData[,] gridMatrix, List<BusLineSaveData> busSequence,
            int passengerCapacity, string savePath, bool isEditMode, int levelId, int time = 45)
        {
            if (gridMatrix == null)
            {
                Debug.LogWarning("[LevelSaveUtility] Grid is null. Cannot save.");
                return;
            }

            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);

            string fileName = isEditMode ? $"Level_{levelId}.asset" : $"Level_{GetNextLevelId(savePath)}.asset";
            string fullPath = $"{savePath}/{fileName}";

            LevelDataSO levelData = isEditMode ? LoadOrCreateAsset(fullPath) : CreateAsset(fullPath);

            levelData.Rows = gridMatrix.GetLength(0);
            levelData.Cols = gridMatrix.GetLength(1);
            levelData.PassengerLineCapacity = passengerCapacity;
            levelData.Time = time;
            levelData.BusSequence = new List<BusLineSaveData>(busSequence);
            levelData.GridCells = new List<CellSaveData>();

            for (int x = 0; x < levelData.Rows; x++)
            for (int y = 0; y < levelData.Cols; y++)
                levelData.GridCells.Add(gridMatrix[x, y]);

            EditorUtility.SetDirty(levelData);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"[LevelSaveUtility] Saved successfully: {fullPath}");
        }

        public static LevelDataSO GetSelectedLevel(int levelNumber, string loadPath)
        {
            string fullPath = $"{loadPath}/Level_{levelNumber}.asset";
            var levelData = AssetDatabase.LoadAssetAtPath<LevelDataSO>(fullPath);

            if (!levelData)
                Debug.LogError($"[LevelSaveUtility] Level_{levelNumber} not found at: {fullPath}");

            return levelData;
        }

        public static bool DeleteSelectedLevel(int levelNumber, string loadPath)
        {
            string fullPath = $"{loadPath}/Level_{levelNumber}.asset";
            var target = AssetDatabase.LoadAssetAtPath<LevelDataSO>(fullPath);

            if (!target)
            {
                Debug.LogWarning($"[LevelSaveUtility] Level_{levelNumber} not found at: {fullPath}");
                return false;
            }

            bool confirmed = EditorUtility.DisplayDialog(
                "Delete Level",
                $"Are you sure you want to delete Level_{levelNumber}? This cannot be undone.",
                "Yes, Delete", "Cancel");

            if (!confirmed) return false;

            bool deleted = AssetDatabase.DeleteAsset(fullPath);
            if (deleted)
            {
                AssetDatabase.Refresh();
                Debug.Log($"[LevelSaveUtility] Deleted: {fullPath}");
            }
            else
            {
                Debug.LogError($"[LevelSaveUtility] Failed to delete: {fullPath}");
            }

            return deleted;
        }

        private static LevelDataSO LoadOrCreateAsset(string fullPath)
        {
            var existing = AssetDatabase.LoadAssetAtPath<LevelDataSO>(fullPath);
            if (existing) return existing;

            Debug.LogError($"[LevelSaveUtility] Edit mode active but file not found: {fullPath}. Creating new.");
            return CreateAsset(fullPath);
        }

        private static LevelDataSO CreateAsset(string fullPath)
        {
            var data = ScriptableObject.CreateInstance<LevelDataSO>();
            AssetDatabase.CreateAsset(data, fullPath);
            return data;
        }

        private static int GetNextLevelId(string path)
        {
            int maxId = 0;
            string[] guids = AssetDatabase.FindAssets("t:LevelDataSO", new[] { path });

            foreach (string guid in guids)
            {
                string fileName = Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(guid));
                if (fileName.StartsWith("Level_") &&
                    int.TryParse(fileName.Replace("Level_", ""), out int id) &&
                    id > maxId)
                {
                    maxId = id;
                }
            }

            return maxId + 1;
        }
    }
}