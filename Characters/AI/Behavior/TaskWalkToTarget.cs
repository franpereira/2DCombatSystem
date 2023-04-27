using System.Collections.Generic;
using Konrad.Characters.AI.Pathfinding;
using UnityEngine;

namespace Konrad.Characters.AI.Behavior
{
    /// <summary>
    /// Make the character move to the position of a target.
    /// </summary>
    public class TaskWalkToTarget : BehaviorNode
    {
        readonly Character _character;
        Character _target;
        bool _targetFound;
        List<PathPoint> _path;


        public TaskWalkToTarget(Character character) => _character = character;

        public override NodeState Evaluate()
        {
            if (_targetFound == false) // Checking null for Unity Objects every frame is expensive. 
            {
                const string dataKey = "targetCharacter";
                _target = GetData(dataKey) as Character;
                if (_target == null) return NodeState.Failure;
                _targetFound = true;
            }

            Vector3 targetPos = _target.transform.position;
            Vector3 characterPos = _character.transform.position;
            _path = PathGrid.Instance.FindPath(_character, targetPos);
            var distanceToTarget = targetPos - characterPos;
            if (distanceToTarget.magnitude < 1.5f || _path.Count == 0)
            {
                _character.Move(Vector2.zero);
                _path = null;
                return NodeState.Success;
            }
            if (_path == null) return NodeState.Failure;
            
            //Showing path in debug lines:
            for (int i = 0; i < _path.Count - 1; i++)
            {
                Debug.DrawLine(_path[i].WorldPosition, _path[i + 1].WorldPosition, Color.white);
            }
            
            float directionX = _path[0].WorldPosition.x - characterPos.x;
            float directionY = PathAnalyzer.HasToJump(_path) ? 1f : 0f;
            _character.Move(new Vector2(directionX, directionY));

            return NodeState.Running;
        }
    }
}