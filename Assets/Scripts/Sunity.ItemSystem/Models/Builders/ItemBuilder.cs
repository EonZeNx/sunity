using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Sunity.ItemSystem.Models.Builders
{
    /// <summary>
    /// Class to construct an item instance.
    /// </summary>
    public class ItemBuilder
    {
        private string _id;
        private string _displayName;
        private string _description;

        public ItemBuilder()
        {
            _id = "";
            _displayName = "";
            _description = "";
        }

        public ItemBuilder WithId(string id)
        {
            _id = id;
            return this;
        }

        public ItemBuilder WithDisplayName(string displayName)
        {
            _displayName = displayName;
            return this;
        }

        public ItemBuilder WithDescription(string description)
        {
            _description = description;
            return this;
        }

        /// <summary>
        /// Build an item.
        /// </summary>
        /// <returns></returns>
        public Item Build()
        {
            ThrowExceptionIfStateInvalid();
            var sprite = Resources.Load<Sprite>($"Items/Sprites/{_id}");
            var model = Resources.Load<GameObject>($"Items/Models/{_id}");

            if(sprite == null)
            {
                throw new IndexOutOfRangeException($"Loading sprite failed. (Items/Sprites/{_id} does not exist)");
            }
            if(model == null)
            {
                throw new IndexOutOfRangeException($"Loading model failed. (Items/Models/{_id} does not exist)");
            }

            return new Item(_id, _displayName, _description, sprite, model);
        }

        /// <summary>
        /// Helper method to check current information.
        /// </summary>
        public void ThrowExceptionIfStateInvalid()
        {
            if (string.IsNullOrEmpty(_id))
            {
                throw new ArgumentException("Id should not be null or empty.", nameof(Item.Id));
            }
            if (string.IsNullOrEmpty(_displayName))
            {
                throw new ArgumentException("Display name should not be null or empty.", nameof(Item.Id));
            }
            if (_description == null)
            {
                throw new ArgumentException("Description should not be null.", nameof(Item.Id));
            }
        }
    }
}
