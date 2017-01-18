using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using BoC.Persistence.SitecoreGlass;
using BoC.Persistence.SitecoreGlass.DataContext;
using BoC.Persistence.SitecoreGlass.Models;
using BoC.Persistence.SitecoreGlass.Profiling;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("BoC.Persistence.Sitecore")]
[assembly: AssemblyDescription("Persistence layer for sitecore using glass.mapper")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("BoC.Persistence.Sitecore")]
[assembly: AssemblyCopyright("Copyright ©  2013")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("8b92f8db-b5be-4cd4-8ea6-b40b99430434")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("4.4.0.1")]
[assembly: AssemblyFileVersion("4.4.0.1")]

[assembly: TypeForwardedTo(typeof(DataContextException))]
[assembly: TypeForwardedTo(typeof(SitecoreBucketDataContext))]
[assembly: TypeForwardedTo(typeof(SitecoreDataContext))]
[assembly: TypeForwardedTo(typeof(SitecoreDataContextDatabaseProvider))]
[assembly: TypeForwardedTo(typeof(SitecoreDataContextIndexSearchContextProvider))]
[assembly: TypeForwardedTo(typeof(ContentSearchContextIndexNameProvider))]
[assembly: TypeForwardedTo(typeof(SitecorePerformanceProfiler))]
[assembly: TypeForwardedTo(typeof(ContextDatabaseProvider))]
[assembly: TypeForwardedTo(typeof(CustomDatabaseProvider))]
[assembly: TypeForwardedTo(typeof(IDatabaseProvider))]
[assembly: TypeForwardedTo(typeof(IIndexNameProvider))]
[assembly: TypeForwardedTo(typeof(IProviderSearchContextProvider))]
[assembly: TypeForwardedTo(typeof(ISitecoreServiceProvider))]
[assembly: TypeForwardedTo(typeof(SitecoreRepository<>))]
[assembly: TypeForwardedTo(typeof(SitecoreServiceProvider))]
[assembly: TypeForwardedTo(typeof(ParametersTemplateValueProviderFactory))]
[assembly: TypeForwardedTo(typeof(ISearchable))]
[assembly: TypeForwardedTo(typeof(ISitecoreItem))]
[assembly: TypeForwardedTo(typeof(SitecoreItem))]
