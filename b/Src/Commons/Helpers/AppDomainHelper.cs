using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BoC.InversionOfControl;

namespace BoC.Helpers
{
    public class AppDomainHelper: IDisposable, IAppDomainHelper
    {
        private readonly string domainPath;
        private readonly string fileFilter;
        private List<Assembly> loadedAssemblies = new List<Assembly>();
        static private readonly object loadlock = new object();

        public AppDomainHelper(string domainPath, string fileFilter)
        {
            Loaded = false;
            this.domainPath = domainPath;
            this.fileFilter = fileFilter;
        }

        public string DomainPath
        {
            get { return domainPath; }
        }

        public string FileFilter
        {
            get { return fileFilter; }
        }
        public bool Loaded { get; private set; }

        public IEnumerable<Assembly> GetAssemblies()
        {
            EnsureLoaded();
            return loadedAssemblies;
        }

        public IEnumerable<Type> GetTypes(Func<Type, bool> where)
        {
            return GetAssemblies().SelectMany(a => a.GetTypes()).Where(where);
        }

        public void Refresh()
        {
            lock (loadlock)
            {
                Loaded = false;
                loadedAssemblies.Clear();
            }
        }

        private void EnsureLoaded()
        {
            if (Loaded) return;

            lock (loadlock)
            {
                if (!Loaded)
                {
                    loadedAssemblies.Clear();
                    if (!String.IsNullOrEmpty(DomainPath))
                    {
                        var paths = DomainPath.Split(';');
                        foreach (var path in paths)
                        {
                            try
                            {
                                loadedAssemblies.AddRange(Directory.GetFiles(path, FileFilter).Select(Assembly.LoadFrom));
                                Loaded = true;
                            }
                                // Files should always exists but don't blow up here if they don't
                            catch (FileNotFoundException)
                            {
                            }
                                // File was found but could not be loaded
                            catch (FileLoadException)
                            {
                            }
                                // Dlls that contain native code are not loaded, but do not invalidate the Directory
                            catch (BadImageFormatException)
                            {
                            }
                                // Dlls that have missing Managed dependencies are not loaded, but do not invalidate the Directory 
                            catch (ReflectionTypeLoadException)
                            {
                            }

                        }

                    }
                }
            }
        }


        /// <summary>
        ///     Releases the unmanaged resources used by the <see cref="AppDomainHelper"/> and 
        ///     optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <see langword="true"/> to release both managed and unmanaged resources; 
        ///     <see langword="false"/> to release only unmanaged resources.
        /// </param>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.loadedAssemblies != null)
                {
                    this.loadedAssemblies.Clear();
                    this.loadedAssemblies = null;
                }
            }
        }


        public static IAppDomainHelper CreateDefault()
        {
            return
                new AppDomainHelper(
                    AppDomain.CurrentDomain.SetupInformation.PrivateBinPath ?? AppDomain.CurrentDomain.BaseDirectory,
                    "*.dll");
        }
    }
}