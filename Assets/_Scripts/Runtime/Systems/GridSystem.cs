using System.Collections.Generic;
using _Scripts.Runtime.Data.UnityObjects;
using _Scripts.Runtime.Data.ValueObjects;
using _Scripts.Runtime.Enums;

namespace _Scripts.Runtime.Systems
{
    public class GridSystem
    {
        private readonly GridNode[,] _grid;
        private readonly int _width;
        private readonly int _height;

        public GridSystem(LevelDataSO levelData)
        {
            _width = levelData.Rows;
            _height = levelData.Cols;
            _grid = new GridNode[_width, _height];

            InitializeLogicalGrid(levelData.GridCells);
        }

        private void InitializeLogicalGrid(List<CellSaveData> cellDataList)
        {
            foreach (var data in cellDataList)
            {
                bool isWalkable = data.occupant == OccupantType.Empty;
                _grid[data.coordinates.x, data.coordinates.y] = new GridNode(data.coordinates.x,
                    data.coordinates.y, isWalkable, data.type, data.occupant);
            }
        }
    }
}