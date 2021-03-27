using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sunity.Inventory.Assets.Scripts.Sunity.Inventory
{
    /// <summary>
    /// Class that holds an immutable list of item definitions.
    /// </summary>
    [CreateAssetMenu(fileName = "ItemDatabase", menuName = "Inventory/ItemDatabase")]
    public class ItemDatabase : ScriptableObject
    {
        [SerializeField]
        private List<Item> _items;

        /// <summary>
        /// Obtain item with the object name <paramref name="objectName"/>.
        /// </summary>
        public Item GetItemById(string objectName)
        {
            return _items.FirstOrDefault(item => item.name == objectName);
        }
    }
}