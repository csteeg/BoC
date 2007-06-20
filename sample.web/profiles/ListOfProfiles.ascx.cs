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
using Diggero.Services;
using Steeg.Framework.Web.UI;
using Steeg.Framework.Web.UI.Helpers;
public partial class profiles_ListOfProfiles : ServicedUserControl<IProfileService>
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        this.DataBinding += new EventHandler(profiles_ListOfProfiles_DataBinding);
    }

    void profiles_ListOfProfiles_DataBinding(object sender, EventArgs e)
    {
        XmlHelper.SetXmlSource(
            xmlControl, 
            this.Service.GetProfiles(this.StartRow, PageSize));
    }

    public int StartRow
    {
        get
        {
            if (Request[this.ClientID + ".startRow"] != null)
            {
                return int.Parse(Request[this.ClientID + ".startRow"]);
            }
            else
            {
                return 0;
            }
        }
    }

    public int PageSize
    {
        get
        {
            if (this.ViewState["PageSize"] == null)
                return 10;
            else
                return (int)this.ViewState["PageSize"];
        }
        set
        {
            this.ViewState["PageSize"] = value;
        }
    }
}
