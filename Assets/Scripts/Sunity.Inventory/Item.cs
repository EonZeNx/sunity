using UnityEngine;

namespace Sunity.Inventory
{
    /// <summary>
    /// Item definition class.
    /// Identified by object name, when instantiated as a ScriptableObject asset.
    /// </summary>
    [CreateAssetMenu(fileName = "Item", menuName = "Inventory/Item")]
    [System.Serializable]
    public class Item : ScriptableObject
    {
        [SerializeField]
        private string _displayName;
        [SerializeField]
        private string _description;
        [SerializeField]
        private Sprite _sprite;
        [SerializeField]
        private GameObject _model;

        public string DisplayName { get => _displayName; }
        public string Description { get => _description; }
        public Sprite Sprite { get => _sprite; }
        public GameObject Model { get => _model; }
    }
}