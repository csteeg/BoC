using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoC.InversionOfControl.SimpleInjector;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Commons.InverionOfControl.SimpleInjector.Tests
{
    [TestClass]
    public class SimpleInjectorResolverFixture
    {
        [TestMethod]
        public void ResolveAll_Should_Return_All_Registed_Types()
        {
            var resolver = new SimpleInjectorDependencyResolver();
            resolver.RegisterType<IFace1, Class1>();
            resolver.RegisterType<IFace1, Class2>();
            resolver.RegisterInstance<IFace1>(new Class3());

            var resolveAll = resolver.ResolveAll<IFace1>();
            Assert.AreEqual(3, resolveAll.Count());
            Assert.IsTrue(resolveAll.OfType<Class1>().Any());
            Assert.IsTrue(resolveAll.OfType<Class2>().Any());
            Assert.IsTrue(resolveAll.OfType<Class3>().Any());
        }

        [TestMethod]
        public void Array_Injector_Should_Inject_All_Registed_Types()
        {
            var resolver = new SimpleInjectorDependencyResolver();
            resolver.RegisterType<IFace1, Class1>();
            resolver.RegisterType<IFace1, Class2>();
            resolver.RegisterInstance<IFace1>(new Class3());

            var resolve = resolver.Resolve<Class4>();
            Assert.AreEqual(3, resolve.Ifaces.Count());
            Assert.IsTrue(resolve.Ifaces.OfType<Class1>().Any());
            Assert.IsTrue(resolve.Ifaces.OfType<Class2>().Any());
            Assert.IsTrue(resolve.Ifaces.OfType<Class3>().Any());
        }

        [TestMethod]
        public void IsRegistered_Should_Return_True()
        {
            var resolver = new SimpleInjectorDependencyResolver();
            resolver.RegisterType<IFace1, Class1>();
            resolver.RegisterType<IFace1, Class2>();
            resolver.RegisterInstance<IFace1>(new Class3());

            var result = resolver.IsRegistered<IFace1>();
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsRegistered_Should_Return_False()
        {
            var resolver = new SimpleInjectorDependencyResolver();
            resolver.RegisterType<IFace1, Class1>();
            resolver.RegisterType<IFace1, Class2>();
            resolver.RegisterInstance<IFace1>(new Class3());

            var result = resolver.IsRegistered<Class3>();
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Unresolveable_Should_Return_Null()
        {
            var resolver = new SimpleInjectorDependencyResolver();

            var result = resolver.Resolve<IFace1>();
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Unresolveable_Should_Return_Default()
        {
            var resolver = new SimpleInjectorDependencyResolver();

            var result = resolver.Resolve<Class3>();
            Assert.IsNotNull(result);
        }
    }

    public interface IFace1 {}
    public class Class1: IFace1 { }
    public class Class2 : IFace1 { }
    public class Class3 : IFace1 { }

    public class Class4
    {
        public IFace1[] Ifaces { get; set; }

        public Class4(IFace1[] ifaces)
        {
            Ifaces = ifaces;
        }
    }
}
