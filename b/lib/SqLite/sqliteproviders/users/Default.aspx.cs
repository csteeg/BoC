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

public partial class users_Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Profile.HomePhone = "21212";
    }
    protected void Button2_Click(object sender, EventArgs e)
    {
        WebPartManager1.Personalization.ResetPersonalizationState();
    }
}
