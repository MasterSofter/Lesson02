using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace Search
{
    public static class AStarFromGoogle
    {
        public static List<Vector2Int> FindPath(int[,] field, Vector2Int start, Vector2Int goal)
        {
            // Шаг 1.
            var closedSet = new Collection<PathNode>();
            var openSet = new Collection<PathNode>();
            // Шаг 2.
            PathNode startNode = new PathNode()
            {
                Position = start,
                CameFrom = null,
                PathLengthFromStart = 0,
                HeuristicEstimatePathLength = GetHeuristicPathLength(start, goal)
            };
            openSet.Add(startNode);
            while (openSet.Count > 0)
            {
                // Шаг 3.
                var currentNode = openSet.OrderBy(node => 
                    node.EstimateFullPathLength).First();
                // Шаг 4.
                if (currentNode.Position == goal)
                    return GetPathForNode(currentNode);
                // Шаг 5.
                openSet.Remove(currentNode);
                closedSet.Add(currentNode);
                // Шаг 6.
                foreach (var neighbourNode in GetNeighbours(currentNode, goal, field))
                {
                    // Шаг 7.
                    if (closedSet.Count(node => node.Position == neighbourNode.Position) > 0)
                        continue;
                    var openNode = openSet.FirstOrDefault(node =>
                        node.Position == neighbourNode.Position);
                    // Шаг 8.
                    if (openNode == null)
                        openSet.Add(neighbourNode);
                    else
                    if (openNode.PathLengthFromStart > neighbourNode.PathLengthFromStart)
                    {
                        // Шаг 9.
                        openNode.CameFrom = currentNode;
                        openNode.PathLengthFromStart = neighbourNode.PathLengthFromStart;
                    }
                }
            }
            // Шаг 10.
            return null;
        }
        
        private static int GetDistanceBetweenNeighbours()
        {
            return 1;
        }
        
        private static int GetHeuristicPathLength(Vector2Int from, Vector2Int to)
        {
            return Math.Abs(from.x - to.x) + Math.Abs(from.y - to.y);
        }
        
        private static Collection<PathNode> GetNeighbours(PathNode pathNode, 
            Vector2Int goal, int[,] field)
        {
            var result = new Collection<PathNode>();
 
            // Соседними точками являются соседние по стороне клетки.
            Vector2Int[] neighbourPoints = new Vector2Int[4];
            neighbourPoints[0] = new Vector2Int(pathNode.Position.x + 1, pathNode.Position.y);
            neighbourPoints[1] = new Vector2Int(pathNode.Position.x - 1, pathNode.Position.y);
            neighbourPoints[2] = new Vector2Int(pathNode.Position.x, pathNode.Position.y + 1);
            neighbourPoints[3] = new Vector2Int(pathNode.Position.x, pathNode.Position.y - 1);
 
            foreach (var point in neighbourPoints)
            {
                // Проверяем, что не вышли за границы карты.
                if (point.x < 0 || point.x >= field.GetLength(0))
                    continue;
                if (point.y < 0 || point.y >= field.GetLength(1))
                    continue;
                // Проверяем, что по клетке можно ходить.
                if ((field[point.x, point.y] != 0) && (field[point.x, point.y] != 1))
                    continue;
                // Заполняем данные для точки маршрута.
                var neighbourNode = new PathNode()
                {
                    Position = point,
                    CameFrom = pathNode,
                    PathLengthFromStart = pathNode.PathLengthFromStart +
                                          GetDistanceBetweenNeighbours(),
                    HeuristicEstimatePathLength = GetHeuristicPathLength(point, goal)
                };
                result.Add(neighbourNode);
            }
            return result;
        }
        
        private static List<Vector2Int> GetPathForNode(PathNode pathNode)
        {
            var result = new List<Vector2Int>();
            var currentNode = pathNode;
            while (currentNode != null)
            {
                result.Add(currentNode.Position);
                currentNode = currentNode.CameFrom;
            }
            result.Reverse();
            return result;
        }
    }
    
    public class PathNode
    {
        // Координаты точки на карте.
        public Vector2Int Position { get; set; }
        // Длина пути от старта (G).
        public int PathLengthFromStart { get; set; }
        // Точка, из которой пришли в эту точку.
        public PathNode CameFrom { get; set; }
        // Примерное расстояние до цели (H).
        public int HeuristicEstimatePathLength { get; set; }
        // Ожидаемое полное расстояние до цели (F).
        public int EstimateFullPathLength {
            get {
                return this.PathLengthFromStart + this.HeuristicEstimatePathLength;
            }
        }
    }
}