using NUnit.Framework;
using Sunity.ItemSystem.Models;
using Sunity.ItemSystem.Tests.TestHelpers;
using System;
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
                return new Inventory();
            }
        }

        [Test]
        public void Inventory_Empty_CanStoreStackableItem()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void Inventory_Empty_CanStoreItemWithDimensions()
        {
            throw new NotImplementedException();
        }
    }
}
