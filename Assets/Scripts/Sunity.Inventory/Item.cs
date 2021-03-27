using UnityEngine;

namespace Sunity.Inventory
{
    /// <summary>
    /// Item definition class.
    /// </summary>
    [CreateAssetMenu(fileName = "Item", menuName = "Inventory/Item")]
    [System.Serializable]
    public class Item : ScriptableObject
    {
        public string displayName;
        public string description;

        public Sprite sprite;
        public GameObject model;
    }
}