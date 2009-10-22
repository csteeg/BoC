using System.Runtime.Serialization;
using BoC.Persistence;

namespace BoC.DomainServices
{
    public interface IRepositoryDomainService<TEntity> where TEntity : class, IBaseEntity {
        void Insert(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        void ValidateEntity(object entity);
    }
}