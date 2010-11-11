using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using BoC.EventAggregator;
using BoC.Helpers;
using BoC.InversionOfControl;
using BoC.Persistence;
using BoC.Services;
using BoC.Services.DefaultSetupTasks;
using BoC.Validation;
using Moq;
using Xunit;

namespace BoC.Tests.Services
{
    public class AutoRegisterModelServicesFixture
    {
        [Fact]
        public void Execute_Should_Register_Known_ModelServices()
        {
            var dependencyResolver = new Mock<TestableDependencyResolver> { CallBase = true };
            var appDomainHelper = new Mock<IAppDomainHelper>();

            appDomainHelper.Setup(a => a.GetTypes(It.IsAny<Func<Type, bool>>()))
                .Returns(() => new[] { typeof(DummyModel), typeof (DummyModelService)});

            var task = new AutoRegisterModelServices(dependencyResolver.Object, new[] {appDomainHelper.Object});
            task.Execute();

            dependencyResolver.Verify(d => d.RegisterType(typeof(IModelService<DummyModel>), typeof(DummyModelService)), Times.Once());
            dependencyResolver.Verify(d => d.RegisterType(It.IsAny<Type>(), It.IsAny<Type>()), Times.Once());
        }

        [Fact]
        public void Execute_Should_Register_ModelServices_That_Have_Inherited_Interface_Implementations()
        {
            var dependencyResolver = new Mock<TestableDependencyResolver> {CallBase = true};
            var appDomainHelper = new Mock<IAppDomainHelper>();

            appDomainHelper.Setup(a => a.GetTypes(It.IsAny<Func<Type, bool>>()))
                .Returns(() => new[] {  typeof(DummyModel2), typeof(DummyModelService2) });

            var task = new AutoRegisterModelServices(dependencyResolver.Object, new[] { appDomainHelper.Object });
            task.Execute();

            dependencyResolver.Verify(d => d.RegisterType(typeof(IModelService<DummyModel2>), typeof(DummyModelService2)), Times.Once());
            dependencyResolver.Verify(d => d.RegisterType(typeof(IDummyModelService2), typeof(DummyModelService2)), Times.Once());
            dependencyResolver.Verify(d => d.RegisterType(It.IsAny<Type>(), It.IsAny<Type>()), Times.Exactly(2));
        }

        [Fact]
        public void Execute_Should_Register_ModelServices_That_Have_Base_ModelService()
        {
            var dependencyResolver = new Mock<TestableDependencyResolver> { CallBase = true };
            var appDomainHelper = new Mock<IAppDomainHelper>();

            appDomainHelper.Setup(a => a.GetTypes(It.IsAny<Func<Type, bool>>()))
                .Returns(() => new[] {  typeof(DummyModel), typeof(InheritedDummyModelService) });

            var task = new AutoRegisterModelServices(dependencyResolver.Object, new[] { appDomainHelper.Object });
            task.Execute();

            dependencyResolver.Verify(d => d.RegisterType(typeof(IModelService<DummyModel>), typeof(InheritedDummyModelService)), Times.Once());
            dependencyResolver.Verify(d => d.RegisterType(It.IsAny<Type>(), It.IsAny<Type>()), Times.Once());
        }

        [Fact]
        public void Execute_Should_Register_ModelServices_For_ModelEntities_That_Dont_Have_A_Service()
        {
            var dependencyResolver = new Mock<TestableDependencyResolver> { CallBase = true };
            var appDomainHelper = new Mock<IAppDomainHelper>();

            appDomainHelper.Setup(a => a.GetTypes(It.IsAny<Func<Type, bool>>()))
                .Returns(() => new[] { typeof(DummyModel), typeof(DummyModel2), typeof(InheritedDummyModelService) });

            var task = new AutoRegisterModelServices(dependencyResolver.Object, new[] { appDomainHelper.Object });
            task.Execute();

            dependencyResolver.Verify(d => d.RegisterType(typeof(IModelService<DummyModel>), typeof(InheritedDummyModelService)), Times.Once());
            dependencyResolver.Verify(d => d.RegisterType(typeof(IModelService<DummyModel2>), It.Is<Type>(value => value.Name.StartsWith("DynamicGeneratedModelService"))), Times.Once());
            dependencyResolver.Verify(d => d.RegisterType(It.IsAny<Type>(), It.IsAny<Type>()), Times.Exactly(2));
        }
        
        [Fact]
        public void Execute_Should_Register_Multiple_ModelServices_For_ModelEntities_That_Dont_Have_A_Service()
        {
            var dependencyResolver = new Mock<TestableDependencyResolver> { CallBase = true };
            var appDomainHelper = new Mock<IAppDomainHelper>();

            appDomainHelper.Setup(a => a.GetTypes(It.IsAny<Func<Type, bool>>()))
                .Returns(() => new[] { typeof(DummyModel), typeof(DummyModel2) });

            var task = new AutoRegisterModelServices(dependencyResolver.Object, new[] { appDomainHelper.Object });
            task.Execute();

            dependencyResolver.Verify(d => d.RegisterType(typeof(IModelService<DummyModel>), It.Is<Type>(value => value.Name.StartsWith("DynamicGeneratedModelService"))), Times.Once());
            dependencyResolver.Verify(d => d.RegisterType(typeof(IModelService<DummyModel2>), It.Is<Type>(value => value.Name.StartsWith("DynamicGeneratedModelService"))), Times.Once());
            dependencyResolver.Verify(d => d.RegisterType(It.IsAny<Type>(), It.IsAny<Type>()), Times.Exactly(2));
        }

        [Fact]
        public void Execute_Should_Register_Multiple_ModelServices_For_ModelEntities_That_Dont_Have_A_Service_With_Custom_Interface()
        {
            var dependencyResolver = new Mock<TestableDependencyResolver> { CallBase = true };
            var appDomainHelper = new Mock<IAppDomainHelper>();

            appDomainHelper.Setup(a => a.GetTypes(It.IsAny<Func<Type, bool>>()))
                .Returns<Func<Type, bool>>(filter => 
                    new[] { typeof(DummyModel), typeof(DummyModel2), typeof(IDummyModelService2) }.Where(filter)
                );

            var task = new AutoRegisterModelServices(dependencyResolver.Object, new[] { appDomainHelper.Object });
            task.Execute();

            dependencyResolver.Verify(d => d.RegisterType(typeof(IModelService<DummyModel>), It.Is<Type>(value => value.Name.StartsWith("DynamicGeneratedModelService"))), Times.Once());
            dependencyResolver.Verify(d => d.RegisterType(typeof(IModelService<DummyModel2>), It.Is<Type>(value => value.Name.StartsWith("DynamicGeneratedModelService"))), Times.Once());
            dependencyResolver.Verify(d => d.RegisterType(typeof(IDummyModelService2), It.Is<Type>(value => value.Name.StartsWith("DynamicGeneratedModelService"))), Times.Once());
            dependencyResolver.Verify(d => d.RegisterType(It.IsAny<Type>(), It.IsAny<Type>()), Times.Exactly(3));
        }


        [Fact]
        public void Execute_Should_Not_Register_ModelServices_For_ModelEntities_If_CreateMissingModelServices_Set_To_False()
        {
            var dependencyResolver = new Mock<TestableDependencyResolver> { CallBase = true };
            var appDomainHelper = new Mock<IAppDomainHelper>();

            appDomainHelper.Setup(a => a.GetTypes(It.IsAny<Func<Type, bool>>()))
                .Returns(() => new[] { typeof(DummyModel), typeof(DummyModel2) });

            var task = new AutoRegisterModelServices(dependencyResolver.Object, new[] { appDomainHelper.Object });
            AutoRegisterModelServices.CreateMissingModelServices = false;
            task.Execute();
            //reset for other tests :)
            AutoRegisterModelServices.CreateMissingModelServices = true;
            dependencyResolver.Verify(d => d.RegisterType(It.IsAny<Type>(), It.IsAny<Type>()), Times.Never());
        }
    }
}