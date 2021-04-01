using Sunity.ItemSystem.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Sunity.ItemSystem
{
    /// <summary>
    /// Class that holds a list of item definitions.
    /// </summary>
    [CreateAssetMenu(fileName = "ItemDatabase", menuName = "Inventory/ItemDatabase")]
    public class ItemDatabase : ScriptableObject
    {
        [SerializeField]
        private List<Item> _items;

        /// <summary>
        /// Set or replace the item database with a new list of items.
        /// </summary>
        /// <param name="items">New list of items</param>
        public void SetAllItems(List<Item> items)
        {
            _items = items;
        }

        /// <summary>
        /// Obtain all items. 
        /// Will not expose to item list, but instead will make a duplicate.
        /// </summary>
        /// <returns>List of all items, duplicated</returns>
        public List<Item> GetAllItems()
        {
            return new List<Item>(_items);
        }

        /// <summary>
        /// Obtain single item by object name <paramref name="id"/>.
        /// </summary>
        /// <param name="id">Object/file name of the item</param>
        /// <returns>Existing item, or null if none exist</returns>
        public Item GetItemById(string id)
        {
            return _items.FirstOrDefault(item => item.Id == id);
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            _items.ForEach(item => 
            {
                stringBuilder.AppendLine(item.Id);
            });
            return stringBuilder.ToString();
        }
    }
}