using System;

namespace BoC.Persistence
{
    public interface IBaseEntity<out TKey> : IBaseEntity
    {
        new TKey Id { get; }
    }

    public interface IBaseEntity : IEquatable<IBaseEntity>
    {
        object Id { get; }
    }
}
