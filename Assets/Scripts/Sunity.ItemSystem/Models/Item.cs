using UnityEngine;

namespace Sunity.ItemSystem.Models
{
    /// <summary>
    /// Item definition class.
    /// </summary>
    public class Item
    {
        public string Id { get; private set; }
        public string DisplayName { get; private set; }
        public string Description { get; private set; }
        public Sprite Sprite { get; private set; }
        public GameObject Model { get; private set; }

        /// <summary>
        /// Specify all properties explicitly.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="displayName"></param>
        /// <param name="description"></param>
        /// <param name="sprite"></param>
        /// <param name="model"></param>
        public Item(string id, string displayName, string description, Sprite sprite, GameObject model)
        {
            Id = id;
            DisplayName = displayName;
            Description = description;
            Sprite = sprite;
            Model = model;
        }
    }
}