using System;

namespace BoC.Persistence
{
    [Serializable]
    abstract public class BaseEntity<Tkey> : IEquatable<BaseEntity<Tkey>>, IBaseEntity<Tkey>
    {
        public virtual Tkey Id { get; set; }

        public virtual bool Equals(BaseEntity<Tkey> obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (GetType() != obj.GetType()) return false;
            return obj.Id.Equals(Id);
        }

        public virtual bool Equals(IBaseEntity other)
        {
            return Equals(other as BaseEntity<Tkey>);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (GetType() != obj.GetType()) return false;
            return Equals(obj as BaseEntity<Tkey>);
        }

        public override int GetHashCode()
        {
            return
                (Id == null || Id.Equals(default(Tkey)) ? base.GetHashCode() : Id.GetHashCode()) ^ GetType().GetHashCode();
        }

        public static bool operator ==(BaseEntity<Tkey> left, BaseEntity<Tkey> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(BaseEntity<Tkey> left, BaseEntity<Tkey> right)
        {
            return !Equals(left, right);
        }

        object IBaseEntity.Id
        {
            get { return Id; }
            set { Id = (Tkey)System.Convert.ChangeType(value, typeof(Tkey)); }
        }
    }
}
