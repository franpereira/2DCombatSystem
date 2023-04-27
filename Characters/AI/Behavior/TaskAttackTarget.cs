using UnityEngine;

namespace Konrad.Characters.AI.Behavior
{
    /// <summary>
    /// Attack the specified target.
    /// </summary>
    public class TaskAttackTarget : BehaviorNode
    {
        readonly Character _character;
        Character _target;
        float _remainingDefenseTime;

        public TaskAttackTarget(Character character) => _character = character;

        public override NodeState Evaluate()
        {
            const string targetKey = "targetCharacter";
            if (ReferenceEquals(_target, null))
            {
                _target = GetData(targetKey) as Character;
                if (ReferenceEquals(_target, null)) return NodeState.Failure;
            }

            if (!_target.IsAlive)
            {
                RemoveData(targetKey);
                return NodeState.Success;
            }

            float targetDirection = _target.transform.position.x - _character.transform.position.x;
            if (targetDirection > 0 && !_character.FacingRight) _character.FaceX(true);
            else if (targetDirection < 0 && _character.FacingRight) _character.FaceX(false);

            _remainingDefenseTime -= Time.deltaTime;
            if (_remainingDefenseTime > 0f)
            {
                _character.Defend(true);
            }
            else
            {
                if (Random.value > 0.01f)
                {
                    _character.Attack();
                }
                else _remainingDefenseTime = 2f;
            }

            return NodeState.Running;
        }
    }
}