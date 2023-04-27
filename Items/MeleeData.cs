using UnityEngine;

namespace Konrad.Items
{
    [CreateAssetMenu(menuName = "Items/Melee Data", fileName = "MeleeData_", order = 0)]
    public class MeleeData : WeaponData
    {
        [Header("Damage HitBox Values")] 
        [SerializeField] float offsetX = 0.5f;
        [SerializeField] float offsetY = 0.0f;
        [SerializeField] float sizeX = 1.6f;
        [SerializeField] float sizeY = 1.0f;

        public float OffsetX => offsetX;
        public float OffsetY => offsetY;
        public float SizeX => sizeX;
        public float SizeY => sizeY;
    }
}