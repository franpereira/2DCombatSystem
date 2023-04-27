using UnityEngine;

namespace Konrad.Items
{
    public abstract class WeaponData : ItemData
    {
        [Header("Weapon Data")]
        [SerializeField] int damage;
        [SerializeField] float attackSpeed;
        
        public int Damage => damage;
        public float AttackSpeed => attackSpeed;
    }
}