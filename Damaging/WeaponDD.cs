using UnityEngine;

namespace Konrad.Damaging
{
    public abstract class WeaponDD : MonoBehaviour, IDamageDealer
    {
        public abstract void DealDamage();

        public abstract void DealDamage(IDamageTaker damageTaker);
    }
}