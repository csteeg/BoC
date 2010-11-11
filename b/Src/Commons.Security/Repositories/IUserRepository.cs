using System;
using System.Collections.Generic;
using System.Text;
using BoC.Persistence;
using BoC.Security.Model;

namespace BoC.Security.Repositories
{
    public interface IUserRepository: IRepository<User>
    {
        User FindByLogin(String login);
        User[] FindByLogin(ICollection<String> logins);
        Int32 CountOnlineUsers(TimeSpan activitySpan);
    }
}