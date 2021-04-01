using NUnit.Framework;
using Sunity.ItemSystem;
using Sunity.ItemSystem.Models;
using Sunity.ItemSystem.Tests.TestHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Sunity.ItemSystem.Tests
{
    [TestFixture]
    public class ItemDatabaseTests
    {
        private class Fixture : IBasicFixture<ItemDatabase>
        {
            private ItemDatabase _itemDatabase;

            public Fixture(params Item[] items)
            {
                _itemDatabase = ScriptableObject.CreateInstance<ItemDatabase>();
                _itemDatabase.SetAllItems(items.ToList());
            }

            public ItemDatabase GetSUT()
            {
                return _itemDatabase;
            }
        }

        /// <summary>
        /// SetAllItems called means that GetAllItems returns a list with the same contents.
        /// </summary>
        [Test]
        public void SetAllItems_ValidItemList_ReturnedListShouldBeRetrievable()
        {
            var expectedItemId = "test";
            var expectedItem = ScriptableObject.CreateInstance<Item>();
            expectedItem.name = expectedItemId;

            var sut = new Fixture().GetSUT(); // Setup fixture with empty list

            sut.SetAllItems(new List<Item>() { expectedItem });

            Assert.AreEqual(expectedItemId, sut.GetAllItems().First().Id);
        }

        /// <summary>
        /// GetAllItems should only return a duplicate.
        /// Tested by editing the list obtained with GetAllItems, another call to GetAllItems should return unedited list.
        /// </summary>
        [Test]
        public void GetAllItems_PopulatedItemList_ReturnedListShouldNotBeSame()
        {
            var expectedItemId = "test";
            var expectedItem = ScriptableObject.CreateInstance<Item>();
            expectedItem.name = expectedItemId;

            var sut = new Fixture(expectedItem).GetSUT(); // Setup fixture with empty list

            sut.GetAllItems().RemoveAll(item => item.Id == expectedItemId);
            var items = sut.GetAllItems(); // Get another list, see if it's been changed

            Assert.AreEqual(expectedItemId, items.First().Id);
        }

        /// <summary>
        /// ToString returns Id of items in the item list.
        /// </summary>
        [Test]
        public void ToString_PopulatedItemListWithOneItem_ReturnsItemId()
        {
            var expectedItemId = "test";
            var expectedItem = ScriptableObject.CreateInstance<Item>();
            expectedItem.name = expectedItemId;

            var sut = new Fixture(expectedItem).GetSUT();

            var databaseString = sut.ToString();

            Assert.AreEqual($"{expectedItemId}\r\n", databaseString);
        }

        /// <summary>
        /// ToString returns all Ids of items in the item list on new lines.
        /// </summary>
        [Test]
        public void ToString_PopulatedItemListWithMultipleItems_ReturnsItemIdsInNewLines()
        {
            var expectedItemId = "test"; // first item
            var expectedItem = ScriptableObject.CreateInstance<Item>();
            expectedItem.name = expectedItemId;

            var expectedItem2Id = "test2"; // second item
            var expectedItem2 = ScriptableObject.CreateInstance<Item>();
            expectedItem2.name = expectedItem2Id;

            var sut = new Fixture(expectedItem, expectedItem2).GetSUT();

            var databaseString = sut.ToString();

            Assert.AreEqual($"{expectedItemId}\r\n{expectedItem2Id}\r\n", databaseString);
        }
    }
}
