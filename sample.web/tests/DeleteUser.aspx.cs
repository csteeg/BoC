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
using Steeg.Framework.Web.UI;

public partial class tests_DeleteUser : ServicedPage<Steeg.Framework.Security.IUserService>
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.Service.DeleteUser(
            this.Service.GetUser(Int64.Parse(Request.QueryString["id"])));
    }
}
