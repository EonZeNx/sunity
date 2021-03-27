using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Sunity.Game.Tests
{
    [TestFixture]
    public class DummyTests
    {
        /// <summary>
        /// Example unit test.
        /// Use the naming scheme; Subject_State_Behaviour,
        /// i.e. 
        /// [Test]
        /// public void AddNumbers_BothInputsPositive_ReturnsPositiveNumber() 
        /// {
        ///     Assert.IsTrue(AddNumbers(1, 1) > 0);
        /// }
        /// ... where the subject is the AddNumbers method
        /// ... the state is that both inputs to AddNumbers are positive
        /// ... and the behaviour is that AddNumbers will return a positive result.
        /// </summary>
        [Test]
        public void Subject_State_Behaviour()
        {
            Assert.AreEqual(true, true);
        }
    }

}