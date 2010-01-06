// Copyright (c) 2009 Chris Pietschmann (http://pietschsoft.com)
// All rights reserved.
// Licensed under the Microsoft Public License (Ms-PL)
// http://opensource.org/licenses/ms-pl.html

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace BoC.Web.Mvc.ScriptManager
{
    public class SimpleScriptManager
    {
        private static string scriptIncludesKey = "BoC.Web.Mvc.ScriptManager.SimpleScriptManager.Includes";
        private static string scriptsKey = "BoC.Web.Mvc.ScriptManager.SimpleScriptManager.Scripts";
        private HtmlHelper htmlHelper;

        private IDictionary<string, string> scriptIncludes
        {
            get
            {
                if (htmlHelper.ViewContext.HttpContext.Items[scriptIncludesKey] == null)
                {
                    htmlHelper.ViewContext.HttpContext.Items[scriptIncludesKey] = new Dictionary<string, string>();
                }
                return htmlHelper.ViewContext.HttpContext.Items[scriptIncludesKey] as IDictionary<string, string>;
                
            }
        }
        private IDictionary<string, string> scripts
        {
            get
            {
                if (htmlHelper.ViewContext.HttpContext.Items[scriptsKey] == null)
                {
                    htmlHelper.ViewContext.HttpContext.Items[scriptsKey] = new Dictionary<string, string>();
                }
                return htmlHelper.ViewContext.HttpContext.Items[scriptsKey] as IDictionary<string, string>;

            }
            
        }
        //private Dictionary<string, Action> scriptsActions = new Dictionary<string, Action>();

        /// <summary>
        /// SimpleScriptManager Constructor
        /// </summary>
        /// <param name="helper">The HtmlHelper that this SimpleScriptManager will use to render to.</param>
        public SimpleScriptManager(HtmlHelper helper)
        {
            // Store reference to the HtmlHelper object this SimpleScriptManager is tied to.
            this.htmlHelper = helper;
        }

        /// <summary>
        /// Adds a script file reference to the page.
        /// </summary>
        /// <param name="scriptPath">The URL of the script file.</param>
        /// <returns>Returns the SimpleScriptManager</returns>
        public SimpleScriptManager ScriptInclude(string scriptPath)
        {
            return this.ScriptInclude(scriptPath.ToLower(), scriptPath);
        }

        /// <summary>
        /// Adds a script file reference to the page.
        /// </summary>
        /// <param name="key">A unique identifier for the script file.</param>
        /// <param name="scriptPath">The URL of the script file.</param>
        /// <returns>Returns the SimpleScriptManager</returns>
        public SimpleScriptManager ScriptInclude(string key, string scriptPath)
        {
            if (!this.scriptIncludes.ContainsKey(key))
            {
                // Check if the scriptPath is a Virtual Path
                if (scriptPath.StartsWith("~/"))
                {
                    // Convert the Virtual Path to an Application Absolute Path
                    scriptPath = VirtualPathUtility.ToAbsolute(scriptPath);
                }
                this.scriptIncludes.Add(key, scriptPath);
            }
            return this;
        }

        /// <summary>
        /// Adds a script file reference to the page for an Embedded Web Resource.
        /// </summary>
        /// <typeparam name="T">The Type whos Assembly contains the Web Resource.</typeparam>
        /// <param name="key">A unique identifier for the script file.</param>
        /// <param name="resourceName">The name of the Web Resource.</param>
        /// <returns>Returns the SimpleScriptManager</returns>
        public SimpleScriptManager ScriptInclude<T>(string key, string resourceName)
        {
            return this.ScriptInclude(key, getWebResourceUrl<T>(resourceName));
        }

        /// <summary>
        /// Adds a script file reference to the page for an Embedded Web Resource.
        /// </summary>
        /// <typeparam name="T">The Type whos Assembly contains the Web Resource.</typeparam>
        /// <param name="resourceName">The name of the Web Resource.</param>
        /// <returns>Returns the SimpleScriptManager</returns>
        public SimpleScriptManager ScriptInclude<T>(string resourceName)
        {
            return this.ScriptInclude(getWebResourceUrl<T>(resourceName));
        }

        /// <summary>
        /// Adds a script block to the page.
        /// </summary>
        /// <param name="javascript">The JavaScript code to include in the Page.</param>
        /// <returns>Returns the SimpleScriptManager</returns>
        public SimpleScriptManager Script(string javascript)
        {
            return this.Script(Guid.NewGuid().ToString(), javascript);
        }

        /// <summary>
        /// Adds a script block to the page.
        /// </summary>
        /// <param name="key">A unique identifier for the script.</param>
        /// <param name="javascript">The JavaScript code to include in the Page.</param>
        /// <returns>Returns the SimpleScriptManager</returns>
        public SimpleScriptManager Script(string key, string javascript)
        {
            if (!this.scripts.ContainsKey(key) && !String.IsNullOrEmpty(javascript))
            {
                this.scripts.Add(key, javascript.Trim());
            }
            return this;
        }

        /// <summary>
        /// Adds a script block to the page.
        /// </summary>
        /// <param name="javascript">The JavaScript code to include in the Page.</param>
        /// <returns>Returns the SimpleScriptManager</returns>
        public SimpleScriptManager Script(Action javascript)
        {
            return this.Script(Guid.NewGuid().ToString(), javascript);
        }

        /// <summary>
        /// Adds a script block to the page.
        /// </summary>
        /// <param name="key">A unique identifier for the script.</param>
        /// <param name="javascript">The JavaScript code to include in the Page.</param>
        /// <returns>Returns the SimpleScriptManager</returns>
        public SimpleScriptManager Script(string key, Action javascript)
        {
            if (!this.scripts.ContainsKey(key))
            {
                var field = javascript.Target.GetType().GetField("__w");
                if (field == null)
                {
                    throw new ArgumentException("Adding script through actions, is only supported from within the ASPXViewEngine", "javascript");
                }

                var oldVal = field.GetValue(javascript.Target);
                try
                {
                    var stringBuilder = new StringBuilder();
                    var stringWriter = new StringWriter(stringBuilder);
                    var newVal = new HtmlTextWriter(stringWriter);
                    field.SetValue(javascript.Target, newVal);
                    javascript();
                    return Script(key, stringBuilder.ToString().Trim());
                }
                finally
                {
                    field.SetValue(javascript.Target, oldVal);
                }
            }
            return this;
        }

        /// <summary>
        /// Renders the SimpleScriptManager to the Page
        /// </summary>
        public void Render()
        {
            var writer = this.htmlHelper.ViewContext.HttpContext.Response.Output;

            // Render All Script Includes to the Page
            foreach (var scriptInclude in this.scriptIncludes)
            {
                writer.WriteLine(String.Format("<script type=\"text/javascript\" src=\"{0}\"></script>", scriptInclude.Value));
            }

            // Render All other scripts to the Page
            if (this.scripts.Count > 0)
            {
                writer.WriteLine("<script type=\"text/javascript\">");

                if (this.scripts.Count > 0)
                {
                    foreach (var script in this.scripts)
                    {
                        writer.Write(script.Value);
                        if (!script.Value.EndsWith(";"))
                        {
                            writer.Write(';');
                        }
                        writer.WriteLine();
                    }
                }

                writer.WriteLine("</script>");
            }
        }


        private static MethodInfo _getWebResourceUrlMethod;
        private static object _getWebResourceUrlLock = new object();

        private static string getWebResourceUrl<T>(string resourceName)
        {
            if (string.IsNullOrEmpty(resourceName))
            {
                throw new ArgumentNullException("resourceName");
            }

            if (_getWebResourceUrlMethod == null)
            {
                lock (_getWebResourceUrlLock)
                {
                    if (_getWebResourceUrlMethod == null)
                    {
                        _getWebResourceUrlMethod = typeof(System.Web.Handlers.AssemblyResourceLoader).GetMethod(
                            "GetWebResourceUrlInternal",
                            BindingFlags.NonPublic | BindingFlags.Static);
                    }
                }
            }

            return "/" + (string)_getWebResourceUrlMethod.Invoke(null,
                                                                 new object[] { Assembly.GetAssembly(typeof(T)), resourceName, false });
        }

    }
}