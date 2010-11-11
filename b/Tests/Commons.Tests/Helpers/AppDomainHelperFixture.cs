using System;
using BoC.Helpers;
using Xunit;

namespace BoC.Tests.Helpers
{
    public class AppDomainHelperFixture
    {
        [Fact]
        public void CreateDefault_Should_Create_Helper_Pointing_To_Current_Bin_Folder()
        {
            using (var helper = AppDomainHelper.CreateDefault() as AppDomainHelper)
            {
                Assert.Equal(AppDomain.CurrentDomain.BaseDirectory, helper.DomainPath);
                Assert.Equal("*.dll", helper.FileFilter);
            }
        }

        [Fact]
        public void GetAssemblies_Should_Get_At_Least_This_Assembly()
        {
            using (var helper = AppDomainHelper.CreateDefault() as AppDomainHelper)
            {
                var assemblies = helper.GetAssemblies();

                Assert.Contains(helper.GetType().Assembly, assemblies);
                Assert.Contains(this.GetType().Assembly, assemblies);
            }
        }

        [Fact]
        public void GetAssemblies_Should_Set_Loaded_To_True()
        {
            using (var helper = AppDomainHelper.CreateDefault() as AppDomainHelper)
            {
                Assert.False(helper.Loaded);
                
                helper.GetAssemblies();

                Assert.True(helper.Loaded);
            }
        }

        [Fact]
        public void Refresh_Should_Set_Loaded_To_False()
        {
            using (var helper = AppDomainHelper.CreateDefault() as AppDomainHelper)
            {
                helper.GetAssemblies();
                Assert.True(helper.Loaded);
                helper.Refresh();
                Assert.False(helper.Loaded);
            }
        }
    }
}