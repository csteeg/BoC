using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace BoC.Security.Model.Mappings
{
    public class RoleMap : IAutoMappingOverride<Role>
    {
        public void Override(AutoMapping<Role> m)
        {
            m.HasManyToMany<User>(r => r.Users).AsSet();
        }
    }
}