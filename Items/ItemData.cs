using UnityEngine;

namespace Konrad.Items
{
    [CreateAssetMenu(menuName = "Items/Item Data", fileName = "ItemData_", order = 0)]
    public class ItemData : ScriptableObject
    {
        [Header("Item Data")]
        [SerializeField] string itemName;
        [SerializeField] Sprite itemIcon;
        
        public string ItemName => itemName;
        public Sprite ItemIcon => itemIcon;
    }
}