using UnityEngine;

namespace Konrad.Characters.Animations.StateMachine
{
    public class AttackAnimState : StateMachineBehaviour
    {
        Character _character;
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_character == null) _character = animator.GetComponentInParent<Character>();
            _character.DefenseEnabled = false;
        }
        
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
                _character.DefenseEnabled = true;
        }
    }
}
