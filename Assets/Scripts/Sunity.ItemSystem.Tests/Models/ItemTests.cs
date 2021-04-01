using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Sunity.ItemSystem.Models;
using UnityEngine;
using UnityEngine.TestTools;

namespace Sunity.ItemSystem.Tests
{
    [TestFixture]
    public class ItemTests
    {
        /// <summary>
        /// The Id should be based on the object/file name of the Item.
        /// </summary>
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
