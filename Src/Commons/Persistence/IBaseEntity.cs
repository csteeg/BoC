using System;

namespace BoC.Persistence
{
    public interface IBaseEntity<TKey> : IBaseEntity
    {
        new TKey Id { get; set; }
    }

    public interface IBaseEntity : IEquatable<IBaseEntity>
    {
        object Id { get; set; }
    }
}
