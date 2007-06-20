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

using Castle.Facilities.NHibernateIntegration;
using Diggero.Model;
using Diggero.Dao;
using Steeg.Framework.Web.UI;

public partial class CreateDB : ServicedPage<IProfileDao>
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Profile p = new Profile();
        p.Age = 21;
        p.BodyType = BodyType.Athletic;
        p.CupSize = "CC";
        p.Description = "Lekker hoorrr";
        p.Ethnicity = Ethnicity.Caucasian;
        p.Fantasy = "Hahaha";
        p.FromProfileSite = new ProfileSite();
        p.FromProfileSite.Name = "Camz.nl";
        p.Gender = Gender.Female;
        p.HasAudio = true;
        p.HasPhone = true;
        p.Nick = "Kell";
        p.RemoteId = 123;
        p.SexualPreference = SexualPreference.BiSexual;

        this.Service.Save(p);
    }
}
