using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sunity.Inventory
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
        /// Obtain item by object name <paramref name="id"/>.
        /// This should be used as the Item Id.
        /// </summary>
        public Item GetItemById(string id)
        {
            return _items.FirstOrDefault(item => item.Id == id);
        }

        public void LogContents()
        {
            _items.ForEach(item => 
            {
                Debug.Log(item.Id);
                Debug.Log(item.DisplayName);
            });
        }
    }
}