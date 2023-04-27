using UnityEngine;

namespace Konrad.Characters.Animations.StateMachine
{
    public class ParryAnimState : StateMachineBehaviour
    {
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool(AnimatorConst.Parry, false);
        }
    }
}
