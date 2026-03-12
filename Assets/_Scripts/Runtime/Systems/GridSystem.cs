using System;
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

        private int ExitRow => _height - 1;
        private const int MoveStraightCost = 10;

        public GridSystem(LevelDataSO levelData)
        {
            int maxX = 0, maxY = 0;
            foreach (var data in levelData.GridCells)
            {
                if (data.coordinates.x > maxX) maxX = data.coordinates.x;
                if (data.coordinates.y > maxY) maxY = data.coordinates.y;
            }

            _width = maxX + 1;
            _height = maxY + 1;
            _grid = new GridNode[_width, _height];

            InitializeGrid(levelData.GridCells);
        }

        private void InitializeGrid(List<CellSaveData> cellDataList)
        {
            foreach (var data in cellDataList)
            {
                // Passenger olan obstructed node'lar aslında walkable
                CellType cellType = data.occupant == OccupantType.Passenger && data.type == CellType.Obstructed
                    ? CellType.Walkable
                    : data.type;

                _grid[data.coordinates.x, data.coordinates.y] =
                    new GridNode(data.coordinates.x, data.coordinates.y, cellType, data.occupant);
            }
        }

        public GridNode GetNode(int x, int y) =>
            x >= 0 && x < _width && y >= 0 && y < _height ? _grid[x, y] : null;

        public void FreeNode(int x, int y) => GetNode(x, y)?.SetOccupant(OccupantType.Empty);

        public List<GridNode> CalculatePathToExit(int startX, int startY) => FindPathToExit(startX, startY);

        private List<GridNode> FindPathToExit(int startX, int startY)
        {
            GridNode startNode = GetNode(startX, startY);
            if (startNode == null) return null;

            ResetGridCosts();

            startNode.GCost = 0;
            startNode.HCost = CalculateDistanceToExit(startNode);

            var openList = new List<GridNode> { startNode };
            var closedList = new HashSet<GridNode>();

            while (openList.Count > 0)
            {
                GridNode current = GetLowestFCostNode(openList);

                if (IsExitNode(current))
                    return RetracePath(startNode, current);

                openList.Remove(current);
                closedList.Add(current);

                foreach (var neighbor in GetNeighbors(current))
                {
                    if (closedList.Contains(neighbor)) continue;

                    if (!neighbor.IsWalkable)
                    {
                        closedList.Add(neighbor);
                        continue;
                    }

                    int tentativeGCost = current.GCost + MoveStraightCost;
                    if (tentativeGCost >= neighbor.GCost) continue;

                    neighbor.CameFromNode = current;
                    neighbor.GCost = tentativeGCost;
                    neighbor.HCost = CalculateDistanceToExit(neighbor);

                    if (!openList.Contains(neighbor))
                        openList.Add(neighbor);
                }
            }

            return null;
        }

        private void ResetGridCosts()
        {
            for (int x = 0; x < _width; x++)
            for (int y = 0; y < _height; y++)
            {
                if (_grid[x, y] == null) continue;
                _grid[x, y].GCost = int.MaxValue;
                _grid[x, y].CameFromNode = null;
            }
        }

        private List<GridNode> GetNeighbors(GridNode node)
        {
            var neighbors = new List<GridNode>();

            if (node.X - 1 >= 0) neighbors.Add(GetNode(node.X - 1, node.Y));
            if (node.X + 1 < _width) neighbors.Add(GetNode(node.X + 1, node.Y));
            if (node.Y - 1 >= 0) neighbors.Add(GetNode(node.X, node.Y - 1));
            if (node.Y + 1 < _height) neighbors.Add(GetNode(node.X, node.Y + 1));

            return neighbors;
        }

        private GridNode GetLowestFCostNode(List<GridNode> nodes)
        {
            GridNode lowest = nodes[0];
            for (int i = 1; i < nodes.Count; i++)
                if (nodes[i].FCost < lowest.FCost)
                    lowest = nodes[i];
            return lowest;
        }

        private List<GridNode> RetracePath(GridNode start, GridNode end)
        {
            var path = new List<GridNode>();
            GridNode current = end;

            while (current != start)
            {
                path.Add(current);
                current = current.CameFromNode;
            }

            path.Reverse();
            return path;
        }

        private bool IsExitNode(GridNode node) => node.Y == ExitRow;
        private int CalculateDistanceToExit(GridNode node) => Math.Abs(ExitRow - node.Y) * MoveStraightCost;
    }
}