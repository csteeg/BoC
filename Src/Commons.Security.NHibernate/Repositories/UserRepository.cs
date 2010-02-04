using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoC.Persistence.NHibernate;
using BoC.Security.Model;
using NHibernate;

namespace BoC.Security.Repositories.NHibernate
{
    public class UserRepository: NHRepository<User>, IUserRepository
    {
        private readonly ISessionManager sessionManager;

        public UserRepository(ISessionManager sessionManager) : base(sessionManager)
        {
            this.sessionManager = sessionManager;
        }

        #region IUserDao Members

        virtual public User FindByLogin(String login)
        {
            return this.FindOne(user => user.Login == login);
        }

        virtual public User[] FindByLogin(ICollection<String> logins)
        {
            if (logins == null || logins.Count == 0)
                return new User[0];
            return this.Query(user => logins.Contains(user.Login));
        }

        virtual public int CountOnlineUsers(TimeSpan activitySpan)
        {
            DateTime compareTime = DateTime.Now.Subtract(activitySpan);
            return
                (from u in this
                 where u.LastActivity > compareTime
                 select u).Count();
        }

        #endregion

    }
}