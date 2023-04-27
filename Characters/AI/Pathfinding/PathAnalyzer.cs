using System.Collections.Generic;

namespace Konrad.Characters.AI.Pathfinding
{
    /// <summary>
    /// Functions that give information about a pathfinding route.
    /// </summary>
    public static class PathAnalyzer
    {
        /// <summary>
        /// Tells if a walking character should jump to get to the following point.
        /// This is the case if the next point has a higher altitude than the current point.
        /// </summary>
        public static bool HasToJump(List<PathPoint> path)
        {
            if (path.Count < 2) return false;
            PathPoint first = path[0];
            PathPoint second = path[1];
            bool hasToJump = first.GridPosition.y - second.GridPosition.y < -0.5f;

            return hasToJump && path.Count >= 4;
        }
    }
}