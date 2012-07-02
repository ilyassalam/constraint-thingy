using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using CSharpUtils.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CSharpUtilsTest
{
    [TestClass]
    public class LLEquality
    {
        [TestMethod]
        public void TestMethod1()
        {
            Assert.IsTrue(LL.Create(1).Equals(LL.Create(1)));

            Assert.IsTrue(LL.Create(1, 2).Equals(LL.Create(1, 2)));

            Assert.IsFalse(LL.Create(1).Equals(LL.Create(2)));

            Assert.IsTrue(LL.Create(1, 2).Equals(LL.Create(1, 2)));

            Assert.IsTrue(LL.Create(1, 2, 3).Equals(LL.Create(1, 2, 3)));

            Assert.IsFalse(LL.Create().Equals(LL.Create(1)));

            Assert.IsTrue(LL.Create().Equals(LL.Create()));

            Assert.IsTrue(LL.Create<Tuple<int>>(null, null, null).Equals(LL.Create<Tuple<int>>(null, null, null)));

            Assert.IsFalse(LL.Create<Tuple<int>>(null, new Tuple<int>(5), null).Equals(LL.Create<Tuple<int>>(null, null, null)));

            Assert.IsFalse(LL.Create<Tuple<int>>(null, null, null).Equals(LL.Create<Tuple<int>>(new Tuple<int>(0), null, null)));

            var sharedTail = LL.Create(1, 2, 3, 4);

            Assert.IsTrue(sharedTail.AddFront(5).Equals(sharedTail.AddFront(5)));

            Assert.IsFalse(sharedTail.AddFront(5).Equals(sharedTail.AddFront(6)));
        }
    }
}
