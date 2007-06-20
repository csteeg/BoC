using System;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;

using Diggero.Services;
using Diggero.Model;

using Steeg.Framework.Web.UI.WebServices;

/// <summary>
/// Summary description for WebService
/// </summary>
[WebService(Namespace = Steeg.Framework.Web.UI.Helpers.XmlHelper.DEFAULT_XML_NAMESPACE)]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class ProfilesWebService : ServicedWebService<IProfileService>
{

    public ProfilesWebService()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public Profile[] GetProfiles()
    {
        return this.Service.GetProfiles();
    }

}

