using UnityEngine;

namespace Konrad.Characters.Animations
{
    public static class AnimatorConst
    {
        public static readonly int Death = Animator.StringToHash("Death");
        public static readonly int Hurt = Animator.StringToHash("Hurt");
        public static readonly int Parry = Animator.StringToHash("Parry");
        public static readonly int AnimState = Animator.StringToHash("AnimState");
        public static readonly int SpeedY = Animator.StringToHash("AirSpeedY");
        public static readonly int Grounded = Animator.StringToHash("Grounded");
        public static readonly int Jump = Animator.StringToHash("Jump");
        public static readonly int DefenseStance = Animator.StringToHash("DefenseStance");
    }
}