using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Castle.Windsor;
using Castle.Facilities.NHibernateIntegration;
using Steeg.Framework.Web;
public partial class CreateDB : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        IWindsorContainer container = Steeg.Framework.SteegWindsorContainer.Obtain();
        NHibernate.Cfg.Configuration c = container["nhibernate.factory.cfg"] as NHibernate.Cfg.Configuration;
        
        NHibernate.Tool.hbm2ddl.SchemaExport se = new NHibernate.Tool.hbm2ddl.SchemaExport( c );
        se.Create(true, true);
    }
}
