using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Konrad.Characters.AI.Pathfinding
{
    /// <summary>
    /// Grid based pathfinding.
    /// Generates a grid of points over an area of the map used to find the shortest path between two of them.
    /// </summary>
    public class PathGrid : MonoBehaviour
    {
        /// <summary>
        /// The grid currently instanced in the scene.
        /// </summary>
        // TODO: Support multiple grids on the same scene.
        public static PathGrid Instance;
        void Awake() => Instance = this;

        [SerializeField] Vector2 size;
        [SerializeField] float pointRadius = 1f;

        PathPoint[,] _points;
        LayerMask _groundLayer;
        LayerMask _unWalkableLayer;

        readonly Dictionary<(PathPoint from, PathPoint to), List<PathPoint>> _pathsCache = new();

        void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(size.x, size.y, 1));
            if (_points == null) return;
            foreach (PathPoint point in _points)
            {
                if (point == null) continue;
                string icon = point.Type switch
                {
                    PathPointType.Walkable => "Walkable.png",
                    PathPointType.UnWalkable => "UnWalkable.png",
                    PathPointType.Ground => "Ground.png",
                    PathPointType.Air => "Air.png",
                    _ => "Null.png"
                };
                Gizmos.DrawIcon(point.WorldPosition, icon, true);
            }
        }

        void Start()
        {
            _groundLayer = LayerMask.GetMask("Ground");
            _unWalkableLayer = LayerMask.GetMask("UnWalkable");
            Vector3 center = transform.position;
            int pointAmountX = Mathf.RoundToInt(size.x / pointRadius);
            int pointAmountY = Mathf.RoundToInt(size.y / pointRadius);
            _points = new PathPoint[pointAmountX, pointAmountY];

            // Creating points:
            for (int x = 0; x < pointAmountX; x++)
            {
                for (int y = 0; y < pointAmountY; y++)
                {
                    Vector2 worldPos = new(center.x - size.x / 2 + pointRadius / 2 + x * pointRadius,
                        center.y - size.y / 2 + pointRadius / 2 + y * pointRadius);
                    int2 gridPos = new(x, y);
                    PathPoint point = new(worldPos, gridPos);
                    _points[x, y] = point;
                }
            }

            // Setting types:
            for (int x = 0; x < pointAmountX; x++)
            {
                for (int y = 0; y < pointAmountY; y++)
                {
                    PathPoint point = _points[x, y];

                    // Setting Neighbours, including diagonals:
                    if (x > 0)
                    {
                        point.Neighbours.Add(_points[x - 1, y]);
                        if (y > 0) point.Neighbours.Add(_points[x - 1, y - 1]);
                        if (y < pointAmountY - 1) point.Neighbours.Add(_points[x - 1, y + 1]);
                    }

                    if (x < pointAmountX - 1)
                    {
                        point.Neighbours.Add(_points[x + 1, y]);
                        if (y > 0) point.Neighbours.Add(_points[x + 1, y - 1]);
                        if (y < pointAmountY - 1) point.Neighbours.Add(_points[x + 1, y + 1]);
                    }

                    if (y > 0) point.Neighbours.Add(_points[x, y - 1]);
                    if (y < pointAmountY - 1) point.Neighbours.Add(_points[x, y + 1]);

                    // Checking for obstacles:
                    if (Physics2D.OverlapCircle(point.WorldPosition, pointRadius / 4, _unWalkableLayer) != null)
                        point.Type = PathPointType.UnWalkable;

                    // Checking for ground:
                    if (Physics2D.OverlapCircle(point.WorldPosition, pointRadius / 4, _groundLayer) != null)
                    {
                        point.Type = PathPointType.Ground;
                        // Setting neighbours right above as walkable:
                        if (y < pointAmountY - 1)
                            _points[x, y + 1].Type = PathPointType.Walkable;
                    }

                    if (point.Type == PathPointType.Null)
                        point.Type = PathPointType.Air;
                }
            }
        }


        public PathPoint GetClosestPointFrom(Character character)
        {
            Vector2 characterPosition = character.transform.position;
            var point = GetClosestPointFrom(characterPosition);
            return point;
        }

        public PathPoint GetClosestPointFrom(Vector2 position)
        {
            if (_points == null) return null;

            PathPoint closestPoint = null;
            float closestDistance = float.MaxValue;
            foreach (PathPoint point in _points)
            {
                if (point == null) continue;
                float distance = Vector2.Distance(position, point.WorldPosition);
                if (distance < closestDistance && point.Type is not PathPointType.UnWalkable and not PathPointType.Ground)
                {
                    closestDistance = distance;
                    closestPoint = point;
                }
            }

            return closestPoint;
        }


        /// <summary>
        /// Manhattan distance between two points in the grid.
        /// </summary>
        public int GridDistance(PathPoint from, PathPoint to)
        {
            int xDist = Mathf.Abs(from.GridPosition.x - to.GridPosition.x);
            int yDist = Mathf.Abs(from.GridPosition.y - to.GridPosition.y);
            return xDist + yDist;
        }

        /// <summary>
        /// Real world distance between two points in the grid.
        /// </summary>
        public float WorldDistance(PathPoint from, PathPoint to)
        {
            return Vector2.Distance(from.WorldPosition, to.WorldPosition);
        }

        /// <summary>
        /// Returns a list of points that form a path from the character to a destination.
        /// The first and last items are the closest points to the character and the destination respectively.
        /// </summary>
        public List<PathPoint> FindPath(Character character, Vector2 destination)
        {
            PathPoint start = GetClosestPointFrom(character);
            PathPoint end = GetClosestPointFrom(destination);
            return FindPath(start, end);
        }
        
        /// <summary>
        /// Returns a list of points that form a path from a starting point to a destination.
        /// </summary>
        //Based on A* pathfinding algorithm.
        public List<PathPoint> FindPath(PathPoint from, PathPoint to)
        {
            // If the path is already cached return it:
            if (_pathsCache.ContainsKey((from, to)))
                return _pathsCache[(from, to)];

            HashSet<PathPoint> openSet = new();
            HashSet<PathPoint> closedSet = new();

            from.CostFromStart = 0;
            from.CostToEnd = 0;
            openSet.Add(from);

            while (openSet.Count > 0)
            {
                // Finding all points that share the same lowest cost:
                LinkedList<PathPoint> lowestCostPoints = new();
                float lowestCost = float.MaxValue;
                foreach (var point in openSet)
                {
                    if (point.TotalCost < lowestCost)
                    {
                        lowestCost = point.TotalCost;
                        lowestCostPoints.Clear();
                        lowestCostPoints.AddFirst(point);
                    }
                    else if (Math.Abs(point.TotalCost - lowestCost) < 0.001f) lowestCostPoints.AddFirst(point);
                }

                // Then choosing the one with the lowest CostToEnd:
                PathPoint current = lowestCostPoints.First.Value;
                foreach (var point in lowestCostPoints)
                {
                    if (point.CostToEnd < current.CostToEnd) current = point;
                }

                // If we reached the end, return the path:
                if (current == to)
                {
                    LinkedList<PathPoint> path = new();
                    while (current != from)
                    {
                        path.AddFirst(current);
                        current = current.Parent;
                    }
                    
                    List<PathPoint> processed =  PathPostProcessor.PostProcess(path);
                    _pathsCache.Add((from, to), processed);
                    return processed;
                }

                // Calculating costs for neighbours:
                foreach (var neighbour in current.Neighbours)
                {
                    if (closedSet.Contains(neighbour)) continue;
                    if (neighbour.Type is PathPointType.UnWalkable or PathPointType.Ground) continue;
                    float costFromStart = current.CostFromStart + WorldDistance(current, neighbour);
                    float costToEnd = WorldDistance(neighbour, to);
                    // Making the heuristic more pessimistic:
                    //costToEnd *= 1.4f;
                    
                    // Air penalty:
                   if (neighbour.Type == PathPointType.Air) costToEnd *= 8f;

                    neighbour.CostFromStart = costFromStart;
                    neighbour.CostToEnd = costToEnd;
                    neighbour.Parent = current;

                    if (!openSet.Contains(neighbour)) openSet.Add(neighbour);
                }

                // All neighbours have been checked, so we can remove the current point from the open set:
                openSet.Remove(current);
                closedSet.Add(current);
            }
            // In case we didn't find a path:
            return null;
        }
    }
}