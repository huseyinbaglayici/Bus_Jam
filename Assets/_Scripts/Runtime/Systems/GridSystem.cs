using System;
using System.Collections.Generic;
using _Scripts.Runtime.Data.UnityObjects;
using _Scripts.Runtime.Data.ValueObjects;
using _Scripts.Runtime.Enums;
using UnityEngine;

namespace _Scripts.Runtime.Systems
{
    public class GridSystem
    {
        private readonly GridNode[,] _grid;
        private readonly int _width;
        private readonly int _height;

        private int YExitRow => _height - 1;
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

            InitializeLogicalGrid(levelData.GridCells);
        }

        private void InitializeLogicalGrid(List<CellSaveData> cellDataList)
        {
            foreach (var data in cellDataList)
            {
                CellType correctedType = data.type;
                if (data.occupant == OccupantType.Passenger && data.type == CellType.Obstructed)
                    correctedType = CellType.Walkable;

                _grid[data.coordinates.x, data.coordinates.y] = new GridNode(
                    data.coordinates.x, data.coordinates.y, correctedType, data.occupant);
            }
        }

        public GridNode GetNode(int x, int y)
        {
            if (x >= 0 && x < _width && y >= 0 && y < _height)
                return _grid[x, y];
            return null;
        }

        public void FreeNode(int x, int y)
        {
            GridNode node = GetNode(x, y);
            if (node == null)
                return;
            node.SetOccupant(OccupantType.Empty);
        }

        public List<GridNode> CalculatePathToExit(int startX, int startY)
        {
            return FindPathToExit(startX, startY);
        }

        private List<GridNode> FindPathToExit(int startX, int startY)
        {
            GridNode startNode = GetNode(startX, startY);
            if (startNode == null) return null;
            List<GridNode> openList = new List<GridNode> { startNode };
            HashSet<GridNode> closedList = new HashSet<GridNode>();
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    GridNode gridNode = _grid[x, y];
                    if (gridNode == null) continue;
                    gridNode.GCost = int.MaxValue;
                    gridNode.CameFromNode = null;
                }
            }

            startNode.GCost = 0;
            startNode.HCost = CalculateDistanceToExit(startNode);

            while (openList.Count > 0)
            {
                GridNode currentNode = GetLowestFCostNode(openList);

                if (IsExitNode(currentNode))
                {
                    return RetracePath(startNode, currentNode);
                }

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                foreach (var neighborNode in GetNeighbors(currentNode))
                {
                    if (closedList.Contains(neighborNode)) continue;

                    if (!neighborNode.IsWalkable)
                    {
                        closedList.Add(neighborNode);
                        continue;
                    }

                    int tentativeGCost = currentNode.GCost + MoveStraightCost;
                    if (tentativeGCost < neighborNode.GCost)
                    {
                        neighborNode.CameFromNode = currentNode;
                        neighborNode.GCost = tentativeGCost;

                        neighborNode.HCost = CalculateDistanceToExit(neighborNode);
                        if (!openList.Contains(neighborNode))
                            openList.Add(neighborNode);
                    }
                }
            }

            return null;
        }

        private List<GridNode> GetNeighbors(GridNode currentNode)
        {
            List<GridNode> neighborList = new List<GridNode>();

            if (currentNode.X - 1 >= 0) neighborList.Add(GetNode(currentNode.X - 1, currentNode.Y)); // Sol
            if (currentNode.X + 1 < _width) neighborList.Add(GetNode(currentNode.X + 1, currentNode.Y)); // Sağ
            if (currentNode.Y - 1 >= 0) neighborList.Add(GetNode(currentNode.X, currentNode.Y - 1)); // Aşağı
            if (currentNode.Y + 1 < _height) neighborList.Add(GetNode(currentNode.X, currentNode.Y + 1)); // Yukarı

            return neighborList;
        }

        private int CalculateDistanceCost(GridNode a, GridNode b)
        {
            int xDistance = Mathf.Abs(a.X - b.X);
            int yDistance = Mathf.Abs(a.Y - b.Y);
            return MoveStraightCost * (xDistance + yDistance);
        }

        private GridNode GetLowestFCostNode(List<GridNode> pathNodeList)
        {
            GridNode lowestFCostNode = pathNodeList[0];
            for (int i = 1; i < pathNodeList.Count; i++)
            {
                if (pathNodeList[i].FCost < lowestFCostNode.FCost)
                {
                    lowestFCostNode = pathNodeList[i];
                }
            }

            return lowestFCostNode;
        }

        private List<GridNode> RetracePath(GridNode startNode, GridNode endNode)
        {
            List<GridNode> path = new List<GridNode>();
            GridNode currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.CameFromNode;
            }

            path.Reverse();
            return path;
        }

        private bool IsExitNode(GridNode node)
        {
            return node.Y == YExitRow;
        }


        private int CalculateDistanceToExit(GridNode node)
        {
            return Math.Abs(YExitRow - node.Y) * MoveStraightCost;
        }
    }
}