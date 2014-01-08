using System;

namespace BoC.Logging
{
    public interface ILogContext : IDisposable
    {
        string Name { get; set; }
    }
}
