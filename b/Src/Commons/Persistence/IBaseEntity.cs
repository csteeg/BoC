using System;

namespace BoC.Persistence
{
    public interface IBaseEntity : IEquatable<IBaseEntity>
    {
        object Id { get; }
    }
}
