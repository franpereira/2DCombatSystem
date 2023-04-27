using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

namespace Konrad.Characters.AI.Pathfinding
{
    public static class PathPostProcessor
    {
        /// <summary>
        /// Processes a path doing some modifications to it.
        /// </summary>
        /// <param name="path">The original path as a LinkedList</param>
        /// <returns>The processed path as a List</returns>
        public static List<PathPoint> PostProcess(LinkedList<PathPoint> path)
        {
            LinkedListNode<PathPoint> node = path.First;
            while (node != null)
            {
                //SpecialCase1(node);
                node = node.Next;
            }
            
            return path.ToList();
        }


        static void SpecialCase1(LinkedListNode<PathPoint> node)
        {
            PathPoint current = node.Value;
            PathPoint previous = node.Previous?.Value;
            PathPoint next = node.Next?.Value;
            if (previous == null || next == null) return;

            int2 currentPos = current.GridPosition;
            int2 previousPos = previous.GridPosition;
            int2 nextPos = next.GridPosition;

            if (currentPos.x < previousPos.x && currentPos.x < nextPos.x)
            {
                Vector2 newWorldPos = new Vector2(current.WorldPosition.x, previous.WorldPosition.y);
                int2 newGridPos = new int2(currentPos.x, previousPos.y);
                PathPoint newPoint = new PathPoint(newWorldPos, newGridPos) { Type = PathPointType.Air };
                current.Parent = newPoint;
                newPoint.Parent = previous;
                node.List.AddBefore(node, newPoint);
            }
        }
    }
}