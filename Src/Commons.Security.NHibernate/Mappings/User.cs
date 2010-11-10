using System;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace BoC.Security.Model.Mappings
{
    public class UserMap : IAutoMappingOverride<User>
    {
        public void Override(AutoMapping<User> m)
        {
            m.HasManyToMany<Role>(u => u.Roles).Cascade.SaveUpdate().AsSet();
        	m.HasMany(u => u.AuthenticationTokens).Cascade.AllDeleteOrphan().AsSet();
            m.IgnoreProperty(u => u.IsOnLine);
            m.IgnoreProperty(u => u.Identity);
        }
    }
}