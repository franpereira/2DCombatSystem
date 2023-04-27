using System.Collections.Generic;
using Konrad.Characters;
using Konrad.Items;
using UnityEngine;

namespace Konrad.Damaging
{
    public class MeleeDD : WeaponDD
    {
        readonly Dictionary<GameObject, IDamageTaker> _targetsOnRange = new();
        GameObject _parent;
        Stats _stats;

        void Awake()
        {
            _parent = transform.parent.gameObject;
            _stats = _parent.GetComponentInParent<Stats>();
        }

        public override void DealDamage(IDamageTaker damageTaker)
        {
            float amount = _stats.Attack;
            DamageEffect effect = new(EffectType.KnockBack, 1.25f);
            Damage damage = new(amount, transform.position, _parent, DamageType.Physical, effect);
            damageTaker.TakeDamage(damage);
        }

        public override void DealDamage()
        {
            foreach (IDamageTaker taker in _targetsOnRange.Values) DealDamage(taker);
        }

        void OnTriggerEnter2D(Collider2D col)
        {
            if (col.TryGetComponent(out IDamageTaker damageTaker))
            {
                _targetsOnRange.Add(col.gameObject, damageTaker);
            }
        }

        void OnTriggerExit2D(Collider2D other)
        {
            _targetsOnRange.Remove(other.gameObject);
        }
    }
}