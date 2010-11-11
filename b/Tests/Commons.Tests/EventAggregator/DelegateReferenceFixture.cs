using System;
using BoC.EventAggregator;
using Xunit;

namespace BoC.Tests.EventAggregator
{
    public class DelegateReferenceFixture
    {
        [Fact]
        public void Keep_Alive_Should_Prevent_Delegate_From_Being_Collected()
        {
            var delegates = new SomeClassHandler();
            var delegateReference = new DelegateReference((Action<string>)delegates.DoEvent, true);

            // ReSharper disable RedundantAssignment
            delegates = null;
            // ReSharper restore RedundantAssignment
            GC.Collect();

            Assert.NotNull(delegateReference.Target);
        }

        [Fact]
        public void Not_Keep_Alive_Should_Allow_Delegate_To_Be_Collected()
        {
            var delegates = new SomeClassHandler();
            var delegateReference = new DelegateReference((Action<string>) delegates.DoEvent, false);

            // ReSharper disable RedundantAssignment
            delegates = null;
            // ReSharper restore RedundantAssignment
            GC.Collect();

            Assert.Null(delegateReference.Target);
        }

        [Fact]
        public void Not_Keep_Alive_Should_Keep_Delegate_If_Still_Alive()
        {
            var delegates = new SomeClassHandler();
            var delegateReference = new DelegateReference((Action<string>)delegates.DoEvent, false);

            GC.Collect();

            Assert.NotNull(delegateReference.Target);

            GC.KeepAlive(delegates);
            // ReSharper disable RedundantAssignment
            delegates = null;
            // ReSharper restore RedundantAssignment
            GC.Collect();

            Assert.Null(delegateReference.Target);
        }

        [Fact]
        public void Target_Should_Return_Action()
        {
            var classHandler = new SomeClassHandler();
            Action<string> myAction = classHandler.MyAction;

            var weakAction = new DelegateReference(myAction, false);

            ((Action<string>) weakAction.Target)("payload");

            Assert.Equal("payload", classHandler.MyActionArg);
        }

        [Fact]
        public void Should_Allow_Collection_Of_Original_Delegate()
        {
            var classHandler = new SomeClassHandler();
            Action<string> myAction = classHandler.MyAction;

            var weakAction = new DelegateReference(myAction, false);

            var originalAction = new WeakReference(myAction);
            // ReSharper disable RedundantAssignment
            myAction = null;
            // ReSharper restore RedundantAssignment
            GC.Collect();
            Assert.False(originalAction.IsAlive);

            ((Action<string>)weakAction.Target)("payload");
            Assert.Equal("payload", classHandler.MyActionArg);
        }

        [Fact]
        public void Should_Return_Null_If_Target_NotAlive()
        {
            SomeClassHandler handler = new SomeClassHandler();
            var weakHandlerRef = new WeakReference(handler);

            var action = new DelegateReference((Action<string>)handler.DoEvent, false);

            // ReSharper disable RedundantAssignment
            handler = null;
            // ReSharper restore RedundantAssignment
            GC.Collect();
            Assert.False(weakHandlerRef.IsAlive);
            Assert.Null(action.Target);
        }

        [Fact]
        public void WeakDelegate_Should_Works_With_Static_Method_Delegates()
        {
            var action = new DelegateReference((Action)SomeClassHandler.StaticMethod, false);

            Assert.NotNull(action.Target);
        }

        [Fact]
        public void Null_Delegate_Should_Throw_Exception()
        {
            Assert.Throws<ArgumentNullException>(() => new DelegateReference(null, true));
        }

        public class SomeClassHandler
        {
            public string MyActionArg;

            public void DoEvent(string value)
            {
                #pragma warning disable 168
                string myValue = value;
                #pragma warning restore 168
            }

            public static void StaticMethod()
            {
                #pragma warning disable 168
                //int i = 0;
                #pragma warning restore 168
            }

            public void MyAction(string arg)
            {
                MyActionArg = arg;
            }
        }
    }
}