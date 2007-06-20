using System;
using System.Collections.Generic;
using System.Text;
using Steeg.Framework.Dao;
using Steeg.Framework.Security;
namespace Steeg.Framework.Dao
{
    public interface IUserDao: IGenericDao<User, long>
    {
        User FindByLogin(String login);
        User[] FindByLogin(ICollection<String> logins);
        User[] FindUsersByPartialPropertyValue(String propertyName, String value, Int32 startRow, Int32 maxRows);
        Int32 CountUsersByPartialPropertyValue(String propertyName, String value);
        Int32 CountOnlineUsers(TimeSpan activitySpan);
        String FindLogin(String propertyName, Object value);
        User FindAuthorizedUser(String login, String password);
    }
}
