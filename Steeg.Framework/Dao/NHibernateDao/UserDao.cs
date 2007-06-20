using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using NHibernate;
using NHibernate.Expression;
using Castle.Facilities.NHibernateIntegration;
using Steeg.Framework.Security;
using Steeg.Framework.Dao.NHibernateDao;

namespace Steeg.Framework.Dao.NHibernateDao
{
    public class UserDao: NHibernateGenericDao<User, Int64>, IUserDao
    {
        public UserDao(ISessionManager sessionManager) : base(sessionManager) { }

        #region IUserDao Members

        virtual public Steeg.Framework.Security.User FindByLogin(String login)
        {
            return this.FindOne("Login", login);
        }

        virtual public Steeg.Framework.Security.User[] FindByLogin(ICollection<String> logins)
        {
            return this.FindByProperty("Login", logins);
        }

        virtual public User[] FindUsersByPartialPropertyValue(String propertyName, String value, Int32 startRow, Int32 maxRows)
        {
            return this.FindAll(new ICriterion[] { Expression.Like(propertyName, value) }, startRow, maxRows);
        }

        virtual public Int32 CountUsersByPartialPropertyValue(String propertyName, String value)
        {
            using (ISession session = GetSession())
            {
                IEnumerator enumerator =
                    session.CreateQuery(
                        String.Format("select count(*) from User as user where user.{0} like ? ", propertyName)
                    ).SetString(0, value).Enumerable().GetEnumerator();
                if (enumerator.MoveNext())
                    return (Int32)enumerator.Current;
                else
                    return 0;
            }
        }

        virtual public User FindAuthorizedUser(string login, string password)
        {
            using (ISession session = GetSession())
            {
                return session.CreateCriteria(typeof(User))
                    .Add( Expression.Eq("Login", login) )
                    .Add( Expression.Eq("Password", password) )
                    .Add( Expression.Eq("IsApproved", true) )
                    .Add( Expression.Eq("IsLockedOut", false) )
                .UniqueResult() as User;
            }
        }

        virtual public int CountOnlineUsers(TimeSpan activitySpan)
        {
            DateTime compareTime = DateTime.Now.Subtract(activitySpan);
            using (ISession session = GetSession())
            {
                IEnumerable results = session.Enumerable(
                    "select count(*) from User where User.LastActivity > ? ", 
                    compareTime,
                    NHibernate.Type.TypeFactory.HeuristicType(typeof(DateTime).AssemblyQualifiedName)
                );
                IEnumerator resulter = results.GetEnumerator();
                resulter.MoveNext();
                return (Int32)resulter.Current;
            }
        }

        virtual public String FindLogin(string propertyName, object value)
        {
            using (ISession session = GetSession())
            {
                IEnumerable results;
                if (value != null)
                {
                    results = session.Enumerable(
                        String.Format("select user.Login from User as user where user.{0} = ? ", propertyName),
                        value,
                        NHibernate.Type.TypeFactory.HeuristicType(value.GetType().AssemblyQualifiedName)
                    );
                }
                else
                {
                    results = session.Enumerable(
                        String.Format("select user.Login from User as user where user.{0} is null ", propertyName));
                }
                IEnumerator resulter = results.GetEnumerator();
                if (resulter.MoveNext())
                    return (String)resulter.Current;
                else
                    return null;
            }
        }

        #endregion

    }
}
