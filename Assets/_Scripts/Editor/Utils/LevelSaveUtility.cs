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
        public static void SaveLevel(CellSaveData[,] gridMatrix, List<BusLineSaveData> busSequance,
            int passengerCapacity, string savePath, bool isEditMode, int levelId, int time = 45)
        {
            if (gridMatrix == null)
                Debug.LogWarning("Grid arg is empty . CANNOT SAVE");

            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);

            LevelDataSO levelData;
            string fileName = isEditMode ? $"Level_{levelId}.asset" : $"Level_{GetNextLevelId(savePath)}.asset";
            string fullDataPath = $"{savePath}/{fileName}";

            if (isEditMode)
            {
                levelData = AssetDatabase.LoadAssetAtPath<LevelDataSO>(fullDataPath);
                if (!levelData)
                {
                    Debug.LogError(
                        $"Edit Mode is ACTIVE but {fullDataPath} CANNOT Found file in PATH Generating New... ");
                    levelData = ScriptableObject.CreateInstance<LevelDataSO>();
                    AssetDatabase.CreateAsset(levelData, fullDataPath);
                }
            }
            else
            {
                levelData = ScriptableObject.CreateInstance<LevelDataSO>();
                AssetDatabase.CreateAsset(levelData, fullDataPath);
            }

            levelData.Rows = gridMatrix.GetLength(0);
            levelData.Cols = gridMatrix.GetLength(1);
            levelData.PassengerLineCapacity = passengerCapacity;
            levelData.Time = time;
            levelData.BusSequence = new List<BusLineSaveData>(busSequance);

            levelData.GridCells = new List<CellSaveData>();
            for (int x = 0; x < levelData.Rows; x++)
            {
                for (int y = 0; y < levelData.Cols; y++)
                {
                    levelData.GridCells.Add(gridMatrix[x, y]);
                }
            }

            EditorUtility.SetDirty(levelData);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"[LevelSaveUtility] Level Saved successfully at : {fullDataPath}");
        }

        public static LevelDataSO GetSelectedLevel(int levelNumber, string loadPath)
        {
            string fullPath = $"{loadPath}/Level_{levelNumber}.asset";
            LevelDataSO loadedLevelData = AssetDatabase.LoadAssetAtPath<LevelDataSO>(fullPath);
            if (!loadedLevelData)
            {
                Debug.LogError(
                    $"[LevelSaveUtility] Level_{levelNumber}.asset CANNOT FOUND! Searched Path : {fullPath}");
            }

            return loadedLevelData;
        }

        private static int GetNextLevelId(string path)
        {
            int maxId = 0;
            string[] guids = AssetDatabase.FindAssets("t:LevelDataSO", new[] { path });
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                string fileName = Path.GetFileNameWithoutExtension(assetPath);
                if (fileName.StartsWith("Level_"))
                {
                    if (int.TryParse(fileName.Replace("Level_", ""), out int id))
                    {
                        if (id > maxId) maxId = id;
                    }
                }
            }

            return maxId + 1;
        }

        public static bool DeleteSelectedLevel(int levelNumber, string loadPath)
        {
            string fullPath = $"{loadPath}/Level_{levelNumber}.asset";
            LevelDataSO targetLevel = AssetDatabase.LoadAssetAtPath<LevelDataSO>(fullPath);
            if (targetLevel == null)
            {
                bool confirm = EditorUtility.DisplayDialog(
                    "Delete Level",
                    $"Are you sure you want to delete Level_{levelNumber}? This action cannot be undone.",
                    "Yes, Delete",
                    "Cancel");

                if (confirm)
                {
                    bool isDeleted = AssetDatabase.DeleteAsset(fullPath);
                    if (isDeleted)
                    {
                        AssetDatabase.Refresh();
                        Debug.Log($"[LevelSaveUtility] Successfully deleted: {fullPath}");
                        return true;
                    }
                    else
                    {
                        Debug.LogError($"[LevelSaveUtility] Failed to delete: {fullPath}");
                        return false;
                    }
                }
            }
            else
            {
                Debug.LogWarning(
                    $"[LevelSaveUtility] Level_{levelNumber}.asset not found at: {fullPath}. CANNOT delete.");
            }

            return false;
        }
    }
}