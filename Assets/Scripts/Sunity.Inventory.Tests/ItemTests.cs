using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Sunity.Inventory.Tests
{
    [TestFixture]
    public class ItemTests
    {
        [Test]
        public void GetItemId_Normal_ShouldReturnObjectName()
        {
            var expectedId = "test";

            var item = ScriptableObject.CreateInstance<Item>();
            item.name = expectedId;

            Assert.AreEqual(expectedId, item.Id);
        }
    }
}
