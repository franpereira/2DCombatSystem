using UnityEngine;

namespace Konrad.Characters.AI.Behavior
{
    /// <summary>
    /// Randomly wander around the map.
    /// </summary>
    // TODO: Make use of the Pathfinding system.
    public class TaskRandomWalking : BehaviorNode
    {
        readonly Character _character;
        float _timer;
        bool _walk;
        int _direction;

        int RandomDirection() => Random.Range(0, 2) * 2 - 1;
        float RandomTime() => Random.Range(0f, 3f);
        
        public TaskRandomWalking(Character character)
        {
            _character = character;
            _timer = RandomTime();
            _direction = RandomDirection();
        }

        public override NodeState Evaluate()
        {
            if (_walk)
            {
                _character.Move(new Vector2(_direction, 0f));
                if (_timer <= 0f)
                {
                    _walk = false;
                    _timer = RandomTime();
                }
            }
            else
            {
                _character.Move(Vector2.zero);
                if (_timer <= 0f)
                {
                    _walk = true;
                    _direction = RandomDirection();
                    _timer = RandomTime();
                }
            }
            _timer -= Time.deltaTime;
            return NodeState.Running;
        }
    }
}