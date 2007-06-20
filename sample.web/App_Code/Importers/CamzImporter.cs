using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using Diggero.Dao;
using Diggero.Model;

using Castle.Services.Logging;

namespace Diggero.Helpers.Importers
{
    public class CamzImporter
    {
        const String SITENAME = "camz.nl";

        IProfileDao profileDao;
        IProfileSiteDao siteDao;
        ILogger logger;
        String xmlUrl;

        public CamzImporter(IProfileDao profileDao, IProfileSiteDao siteDao, String xmlUrl, ILogger logger)
        {
            this.profileDao = profileDao;
            this.siteDao = siteDao;
            this.xmlUrl = xmlUrl;
            this.logger = logger;
        }

        public void Import()
        {
            logger.Debug("Beginning import of camz.nl");
            ProfileSite site = siteDao.FindByName(SITENAME);
            if (site == null)
            {
                site = new ProfileSite();
                site.Name = SITENAME;
                this.siteDao.Create(site);
                logger.Debug("Created new camz site");
            }

            XmlTextReader reader = new XmlTextReader(this.xmlUrl);
            if (logger.IsDebugEnabled) 
                logger.Info("Importing " + xmlUrl);

            reader.Read();
            reader.ReadToDescendant("performers");
            while (reader.ReadToFollowing("performer"))
            {
                Profile profile;
                reader.MoveToElement();
                reader.ReadToDescendant("id");
                reader.MoveToElement();
                Int32 profId = reader.ReadElementContentAsInt();
                if ((profile = this.profileDao.FindByRemoteId(site, profId)) == null)
                    profile = new Profile();
                profile.RemoteId = profId;
                profile.FromProfileSite = site;
                if (logger.IsDebugEnabled)
                    logger.Debug("Importing profile with id " + profId);

                reader.ReadToNextSibling("nickname");
                reader.MoveToElement();
                profile.Nick = reader.ReadElementContentAsString();

                reader.ReadToNextSibling("sexe");
                reader.MoveToElement();
                String sexe = reader.ReadElementContentAsString();
                profile.Gender = (sexe.ToLower() == "f") ? Gender.Female : Gender.Male;

                reader.ReadToNextSibling("age");
                reader.MoveToElement();
                profile.Age = reader.ReadElementContentAsInt();

                reader.ReadToNextSibling("description");
                reader.MoveToElement();
                profile.Description = reader.ReadElementContentAsString();

                reader.ReadToNextSibling("fantasy");
                reader.MoveToElement();
                profile.Fantasy = reader.ReadElementContentAsString();

                reader.ReadToNextSibling("sexpref");
                reader.MoveToElement();
                profile.SexualPreference = ParseSexPref(reader.ReadElementContentAsString());

                reader.ReadToNextSibling("looks");
                reader.MoveToElement();
                profile.Ethnicity = ParseEthnicity(reader.ReadElementContentAsString());

                reader.ReadToNextSibling("stature");
                reader.MoveToElement();
                profile.BodyType = ParseBodyType(reader.ReadElementContentAsString());

                reader.ReadToNextSibling("cupsize");
                reader.MoveToElement();
                String cupsize = reader.ReadElementContentAsString();
                if ((cupsize != null) && (cupsize.Length <= 2))
                    profile.CupSize = cupsize;
                else if ((logger.IsInfoEnabled) 
                    && !cupsize.Equals("onbekend", StringComparison.CurrentCultureIgnoreCase)
                    && !cupsize.Equals("choose", StringComparison.CurrentCultureIgnoreCase)
                    && !cupsize.Equals("kies", StringComparison.CurrentCultureIgnoreCase)
                    )
                    logger.Info("Found cupsize larger then 2 chars: " + cupsize);


                reader.ReadToNextSibling("belcamz");
                reader.MoveToElement();
                profile.HasPhone = (reader.ReadElementContentAsInt() == 1);

                reader.ReadToNextSibling("wav");
                reader.MoveToElement();
                profile.HasAudio = (reader.ReadElementContentAsInt() == 1);

                profileDao.Save(profile);
            }

            reader.Close();
        }

        public Ethnicity ParseEthnicity(string camzVersion)
        {
            if (camzVersion == null)
                return Ethnicity.Other;

            switch (camzVersion.ToLower())
            {
                case "europees":
                    return Ethnicity.Caucasian;
                case "latijns":
                case "latino":
                    return Ethnicity.Latino;
                case "aziatisch":
                    return Ethnicity.Asian;
                case "oost europees":
                    return Ethnicity.MiddleEastern;
                case "negroide":
                    return Ethnicity.African;
                case "onbekend":
                case "":
                    return Ethnicity.Other;
                default:
                    if (logger.IsInfoEnabled)
                        logger.Info("Found unknown Ethnicity from camz.nl: " + camzVersion);
                    return Ethnicity.Other;
            }
        }

        public SexualPreference ParseSexPref(string camzVersion)
        {
            if (camzVersion == null)
            {
                return SexualPreference.Hetero;                
            }

            switch (camzVersion.ToLower())
            {
                case "bisexueel":
                    return SexualPreference.BiSexual;
                case "homo":
                case "gay":
                    return SexualPreference.Gay;
                case "hetero":
                case "onbekend":
                case "":
                    return SexualPreference.Hetero;
                default:
                    if (logger.IsInfoEnabled)
                        logger.Info("Found unknown SexualPreference from camz.nl: " + camzVersion);
                    return SexualPreference.Hetero;
            }
        }

        public BodyType ParseBodyType(string camzVersion)
        {
            if (camzVersion == null)
            {
                return BodyType.Average;
            }

            switch (camzVersion.ToLower())
            {
                case "slank":
                    return BodyType.Slender;
                case "mollig":
                    return BodyType.AFewExtraPounds;
                case "gespierd":
                    return BodyType.Muscled;
                case "normaal":
                case "":
                    return BodyType.Average;
                default:
                    if (logger.IsInfoEnabled)
                        logger.Info("Found unknown BodyType from camz.nl: " + camzVersion);
                    return BodyType.Average;
            }
        }

    }
}
