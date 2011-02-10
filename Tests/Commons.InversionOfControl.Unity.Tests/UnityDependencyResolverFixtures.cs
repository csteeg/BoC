using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoC.InversionOfControl.Unity;
using Xunit;

namespace Commons.InverionOfControl.Unity.Tests
{
    public class UnityDependencyResolverFixture
    {
        [Fact]
        public void ResolveAll_Should_Return_All_Registed_Types()
        {
            var resolver = new UnityDependencyResolver();
            resolver.RegisterType<IFace1, Class1>();
            resolver.RegisterType<IFace1, Class2>();
            resolver.RegisterInstance<IFace1>(new Class3());

            var resolveAll = resolver.ResolveAll<IFace1>();
            Assert.Equal(3, resolveAll.Count());
            Assert.True(resolveAll.OfType<Class1>().Any());
            Assert.True(resolveAll.OfType<Class2>().Any());
            Assert.True(resolveAll.OfType<Class3>().Any());
        }

        [Fact]
        public void Array_Injector_Should_Inject_All_Registed_Types()
        {
            var resolver = new UnityDependencyResolver();
            resolver.RegisterType<IFace1, Class1>();
            resolver.RegisterType<IFace1, Class2>();
            resolver.RegisterInstance<IFace1>(new Class3());

            var resolve = resolver.Resolve<Class4>();
            Assert.Equal(3, resolve.Ifaces.Count());
            Assert.True(resolve.Ifaces.OfType<Class1>().Any());
            Assert.True(resolve.Ifaces.OfType<Class2>().Any());
            Assert.True(resolve.Ifaces.OfType<Class3>().Any());
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
