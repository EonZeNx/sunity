using NUnit.Framework;
using Sunity.ItemSystem.Models;
using Sunity.ItemSystem.Tests.TestHelpers;
using UnityEngine;

namespace Sunity.ItemSystem.Tests
{
    [TestFixture]
    public class InventoryTests
    {
        private class Fixture : IBasicFixture<Inventory>
        {
            public Inventory GetSUT()
            {
                throw new System.NotImplementedException();
            }
        }

        [Test]
        public void Inventory_Empty_CanStoreItemWithQuantity()
        {

        }

        [Test]
        public void Inventory_Empty_CanStoreItemWithDimensions()
        {

        }
    }
}
