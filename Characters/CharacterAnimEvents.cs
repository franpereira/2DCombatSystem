using UnityEngine;

namespace Konrad.Characters
{
    public class CharacterAnimEvents : MonoBehaviour
    {
        Character _character;
        Animator _animator;
        AudioManager _audio;
        
        void Start()
        {
            _character = GetComponentInParent<Character>();
            _audio = AudioManager.Instance;
        }

        // Animation Events
        // These functions are called inside the animation files
        void AE_runStop()
        {
            _audio.PlaySound("RunStop");
        }

        void AE_footstep()
        {
            _audio.PlaySound("Footstep");
        }

        void AE_Jump()
        {
            _audio.PlaySound("Jump");
        }

        void AE_Landing()
        {
            _audio.PlaySound("Landing");
        }

        void AE_Parry()
        {
            _audio.PlaySound("Parry");
        }
        
        void AE_ParryStance()
        {
            //m_audioManager.PlaySound("DrawSword");
        }
        

        void AE_Hurt()
        {
            _audio.PlaySound("Hurt");
        }

        void AE_Death()
        {
            _audio.PlaySound("Death");
        }

        void AE_SwordAttack()
        {
            _audio.PlaySound("SwordAttack");
        }
        
        void AE_SheathSword()
        {
            _audio.PlaySound("SheathSword");
        }
        
    }
}
