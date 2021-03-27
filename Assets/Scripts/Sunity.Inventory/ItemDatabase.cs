using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sunity.Inventory.Assets.Scripts.Sunity.Inventory
{
    /// <summary>
    /// Singleton class that allows registering and retrieval of items.
    /// </summary>
    public class ItemDatabase : ScriptableObject
    {
        private ICollection<Item> Items { get; set; }

        public ItemDatabase()
        {
            Items = LoadItemsFromResources();
            Debug.Log(Items.First());
        }

        public ICollection<Item> LoadItemsFromResources()
        {
            return Resources.LoadAll<Item>("Items");
        }

        /// <summary>
        /// Obtain item with the object name <paramref name="id"/>.
        /// </summary>
        public Item GetItemById(string id)
        {
            return Items.FirstOrDefault(item => item.name == id);
        }
    }
}