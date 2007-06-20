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

public partial class tests_ShowUser : Steeg.Framework.Web.UI.ServicedPage<Steeg.Framework.Security.IUserService>
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            this.DataBind();
    }

    Steeg.Framework.Security.User[] user;
    public Steeg.Framework.Security.User[] MyUser
    {
        get
        {
            if (user == null)
            {
                user = new Steeg.Framework.Security.User[] 
                {
                    this.Service.GetUser(Int64.Parse(Request.QueryString["id"]))
                };
            }
            return user;
        }
    }
}
