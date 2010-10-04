using Norm;

namespace BoC.Persistence.Norm
{
    public class DefaultSessionFactory: ISessionFactory
    {
        //this looks for a connection string in your Web.config - you can override this if you want
        public static string ConnectionString = "MongoDB";
        public IMongo CreateSession()
        {
            return Mongo.Create(ConnectionString);
        }
    }
}
