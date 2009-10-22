using System;
using System.Collections;
using System.Text;
using System.Web.Mvc;
using System.Web.UI;
using System.Collections.Generic;
using Microsoft.Web.Mvc;

namespace System.Web.Mvc
{
    public static class AjaxHelperExtensions
    {
        public static string RegisterJqueryMvc(this AjaxHelper helper, ClientScriptManager scriptManager)
        {
            //TODO: find out how to do this without a scriptmanager, those damned classes are all sealed & internal
            return string.Format("\n<script src=\"{0}\" type=\"text/javascript\"></script>\n<script src=\"{1}\" type=\"text/javascript\"></script>\n<script src=\"{2}\" type=\"text/javascript\"></script>\n",
				scriptManager.GetWebResourceUrl(typeof(JqueryMvc.BootStrapper), "JqueryMvc.ClientScripts.jquery-1.2.6.min.js"),
                scriptManager.GetWebResourceUrl(typeof(JqueryMvc.BootStrapper), "JqueryMvc.ClientScripts.history.js"),
                scriptManager.GetWebResourceUrl(typeof(JqueryMvc.BootStrapper), "JqueryMvc.ClientScripts.mvc.js"));
        }

        /*public static string SubmitButton(this AjaxHelper helper, string name, string text, string formSelector, string targetSelector)
        {
            return ButtonBuilder.SubmitButton(name, text, 
				new { onClick = String.Format("jQuery('{0}').mvcAjaxForm('{1}'); return false;", targetSelector, formSelector) }.ToDictionary()
			).ToString();
        }*/

        public static string JsonSubmitButton(this AjaxHelper helper, string name, string text, string formSelector, string callBack)
        {
			return ButtonBuilder.SubmitButton(name, text,
                new { onClick = String.Format("jQuery('{0}').mvcJsonForm({{onSuccess: {1}}}); return false;", formSelector, callBack) }.ToDictionary()
			).ToString();
        }
    }
}
