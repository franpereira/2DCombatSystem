using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Konrad.Characters.AI.Pathfinding
{
    /// <summary>
    /// Point over a grid used for pathfinding.
    /// </summary>
    public class PathPoint
    {
        public PathPoint(Vector2 worldPos, int2 gridPos)
        {
            WorldPosition = worldPos;
            GridPosition = gridPos;
            Type = PathPointType.Null;
        }
        
        
        // The world position of the point on the scene. 
        public readonly Vector2 WorldPosition;
        
        // The position of the point in the grid.
        public readonly int2 GridPosition;

        // The type of terrain the point is on.
        public PathPointType Type;
        
        // The adjacent points on the grid.
        public readonly List<PathPoint> Neighbours = new(8);
        
        // If this point is being used in the pathfinding algorithm
        // the parent is the point that was used to get to this one.
        internal PathPoint Parent;
        
        // Cost of moving from the starting point to this one.
        internal float CostFromStart;
        
        // Heuristic cost of moving from this point to the destination.
        internal float CostToEnd;
        
        // Heuristic cost of moving from the starting point to the destination passing through this point.
        internal float TotalCost => CostFromStart + CostToEnd;
    }


    /// <summary>
    /// The type of terrain over which a point is located.
    /// </summary>
    public enum PathPointType
    {
        Null,
        Walkable,
        UnWalkable,
        Ground,
        Air
    }
}