using System.Collections;
using System.Collections.Generic;
using Konrad.Characters.Animations;
using Konrad.Damaging;
using UnityEngine;

namespace Konrad.Characters
{
    /// <summary>
    /// Responsible for handling the incoming damage to a character.
    /// </summary>
    public class CharacterDT : MonoBehaviour, IDamageTaker
    {
        Character _c;
        bool _canParry;

        void Awake() => _c = GetComponentInParent<Character>();

        public void TakeDamage(Damage damage)
        {
           if(_c.enabled) StartCoroutine(_takeDamage(damage));
        }

        IEnumerator _takeDamage(Damage damage)
        {
            Vector2 direction = damage.DirectionTo(_c.transform.position);
            bool facingThat = Vector2.Dot(direction, _c.FacingRight ? Vector2.right : Vector2.left) > 0;
            float recievedDamage = damage.Amount;
            // If Blocking:
            if (_c.InDefensePosture && facingThat)
            {
                // Parry:
                yield return StartCoroutine(ParryTimeWindow());
                if (_canParry)
                {
                    var damageTaker = damage.Source.GetComponentInChildren<IDamageTaker>();
                    if (damageTaker != null)
                    {
                        Parry(damageTaker, direction);
                        _canParry = false;
                        yield break;
                    }
                }
                recievedDamage *= 0.35f; // TO-DO: Use stats
                AudioManager.Instance.PlaySound("Block");
            }
            
            _c.Health -= Mathf.RoundToInt(recievedDamage);

            foreach (DamageEffect effect in damage.Effects)
            {
                switch (effect.Type)
                {
                    case EffectType.KnockBack:
                        _c.Rigidbody.AddForce(new Vector2(-direction.x * effect.Value, 0f), ForceMode2D.Impulse);
                        break;
                    case EffectType.Stun:
                        _c.Stun(effect.Value);
                        _c.Animator.SetTrigger(AnimatorConst.Hurt);
                        break;
                    default:
                        Debug.LogError($"DamageEffect {effect.ToString()} not implemented");
                        break;
                }
            }
            AudioManager.Instance.PlaySound("Hurt");
            
            //DebugLog
            string effectsString = "";
            foreach (var effect in damage.Effects)
            {
                effectsString += effect + " ";
            }
            Debug.Log($"{_c.name} took {recievedDamage} damage. Effects: {effectsString} Health: {_c.Health}");
        }
        
        void Parry(IDamageTaker damageTaker, Vector2 direction)
        {
            _c.InDefensePosture = false;
            _c.Animator.SetTrigger(AnimatorConst.Parry);
            _c.Rigidbody.AddForce(-direction * 1f, ForceMode2D.Impulse);
            DamageEffect knockBack = new(EffectType.KnockBack, 5f);
            DamageEffect stun = new(EffectType.Stun, 2f);
            List<DamageEffect> effects = new() { knockBack, stun };
            damageTaker.TakeDamage(new Damage(0f, transform.position, gameObject, DamageType.Physical, effects));
        }

        IEnumerator ParryTimeWindow()
        {
            yield return new WaitForSeconds(0.1f);
            if (_c.InDefensePosture && _c.TimeInDefensePosture < 0.35f) _canParry = true;
        }
    }
}