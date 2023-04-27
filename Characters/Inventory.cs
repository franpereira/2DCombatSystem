using System;
using Konrad.Damaging;
using Konrad.Items;
using UnityEngine;

namespace Konrad.Characters
{
    /// <summary>
    /// Items that belong to a character.
    /// </summary>
    public class Inventory : MonoBehaviour
    {
        [SerializeField] WeaponData startingWeapon;
        [SerializeField] GameObject meleeDDPrefab;
        [SerializeField] GameObject rangedDDPrefab;

        Stats _stats;
        
        void Awake()
        {
            _stats = GetComponent<Stats>();
            EquipWeapon(startingWeapon);
        }
        
        public (WeaponData Data, WeaponDD DD) EquippedWeapon { get; private set; }
        public void EquipWeapon(WeaponData weapon)
        {
            if (EquippedWeapon.DD != null) Destroy(EquippedWeapon.DD);

            switch (weapon)
            {
                case MeleeData data:
                    SetupMeleeWeapon(data);
                    break;
                default:
                    throw new NotImplementedException("Weapon Data type not implemented. Could not instantiate Damage Dealer");
            }
        }
        
        void SetupMeleeWeapon(MeleeData weapon)
        {
            GameObject ddObject = Instantiate(meleeDDPrefab, transform);
            var c = ddObject.GetComponent<BoxCollider2D>();
            c.offset = new Vector2(weapon.OffsetX, weapon.OffsetY);
            c.size = new Vector2(weapon.SizeX, weapon.SizeY);
            
            EquippedWeapon = (weapon, ddObject.GetComponent<MeleeDD>());
            _stats.Attack = weapon.Damage;
        }
    }
}