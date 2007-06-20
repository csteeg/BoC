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

using Diggero.Dao;
using Diggero.Model;
using Diggero.Services;
using Diggero.Helpers.Importers;
using Steeg.Framework.Web.UI;

public partial class imports_camz : ServicedPage<IProfileSiteService>
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string downloadUrl = System.Configuration.ConfigurationManager.AppSettings["camzimporter.profiles-url"];

        CamzImporter camzImporter = new CamzImporter(
            this.Get<IProfileDao>(), 
            this.Get<IProfileSiteDao>(), 
            downloadUrl,
            this.Get<Castle.Services.Logging.ILogger>());
        camzImporter.Import();
    }
}
