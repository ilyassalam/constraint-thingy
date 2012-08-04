using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConstraintThingy
{
    [TestClass]
    public class VariableTests
    {
        [TestMethod]
        public void ConstructorTest()
        {
            var x = new Variable<int>("x", 0);
            Assert.AreEqual(0, x.Value);
        }

        [TestMethod]
        public void AssignmentTest()
        {
            var x = new Variable<int>("x", 0);
            x.SetValueOrThrowException(1, null);
            Assert.AreEqual(1, x.Value);
        }

        [TestMethod]
        public void SaveRestoreTest()
        {
            Variable.ResetVariableSystemForTesting();
            var x = new Variable<int>("x", 0);
            x.SetValueOrThrowException(1, null);
            // The abolve assignment should not have caused a spill.
            Assert.AreEqual(0, Variable.StackDepth);
            var frame = Variable.SaveValues();
            x.SetValueOrThrowException(2, null);
            Assert.AreEqual(2, x.Value);
            // The above should have spilled the stack.
            Assert.AreEqual(1, Variable.StackDepth);
            x.SetValueOrThrowException(3, null);
            Assert.AreEqual(3, x.Value);
            // The above should *not* have spilled the stack.
            Assert.AreEqual(1, Variable.StackDepth);

            Variable.RestoreValues(frame);
            Assert.AreEqual(1, x.Value);
            // And now we should be back to an empty stack
            Assert.AreEqual(0, Variable.StackDepth);

            //
            //  Now let's see if we can do it again
            //
            x.SetValueOrThrowException(2, null);
            Assert.AreEqual(2, x.Value);
            // The above should have spilled the stack.
            Assert.AreEqual(1, Variable.StackDepth);
            x.SetValueOrThrowException(3, null);
            Assert.AreEqual(3, x.Value);
            // The above should *not* have spilled the stack.
            Assert.AreEqual(1, Variable.StackDepth);

            Variable.RestoreValues(frame);
            Assert.AreEqual(1, x.Value);
            // And now we should be back to an empty stack
            Assert.AreEqual(0, Variable.StackDepth);

        }
    }
}
