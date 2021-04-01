using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunity.Inventory.Tests.TestHelpers
{
    /// <summary>
    /// <see cref="IBasicFixture{T}"/> will allow for setup of a system/class for the purposes of unit testing.
    /// </summary>
    /// <typeparam name="T">System under test</typeparam>
    public interface IBasicFixture<T>
    {
        /// <summary>
        /// Get system under test. 
        /// </summary>
        /// <returns></returns>
        public T GetSUT();
    }
}
