using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

namespace Commons.Web.Mvc.PrecompiledViews
{
	internal delegate WebPageRenderingBase StartPageLookupDelegate(WebPageRenderingBase page, string fileName, IEnumerable<string> supportedExtensions);


	public class CompiledWebViewPageView : IView
	{
		private readonly string viewPath;
		private readonly WebViewPage page;
		// Methods
		public CompiledWebViewPageView(ControllerContext controllerContext,
			string viewPath,
			string layoutPath,
			bool runViewStartPages,
			IEnumerable<string> viewStartFileExtensions,
			WebViewPage page)
		{
			this.viewPath = viewPath;
			this.page = page;
			this.LayoutPath = layoutPath ?? string.Empty;
			this.RunViewStartPages = runViewStartPages;
			this.StartPageLookup = StartPage.GetStartPage;
			this.ViewStartFileExtensions = viewStartFileExtensions ?? Enumerable.Empty<string>();
		}

		public void Render(ViewContext viewContext, TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (page == null)
			{
				throw new InvalidOperationException("Cannot create a page from " + viewPath);
			}
			//damn, now why is this internal, how can you ever reuse the webviewpage then??
			//page.OverridenLayoutPath = this.LayoutPath;
			if (!String.IsNullOrEmpty(this.LayoutPath))
				page.Layout = LayoutPath;

			page.VirtualPath = viewPath;
			page.ViewContext = viewContext;
			page.ViewData = viewContext.ViewData;
			page.InitHelpers();
			WebPageRenderingBase startPage = null;
			if (this.RunViewStartPages)
			{
				startPage = this.StartPageLookup(page, VirtualPathFactoryManagerViewEngine.ViewStartFileName, this.ViewStartFileExtensions);
			}
			var httpContext = viewContext.HttpContext;
			WebPageRenderingBase base4 = null;
			object model = null;
			
			//instead of page.OverridenLayoutPath:
			if (!String.IsNullOrEmpty(this.LayoutPath))
				page.Layout = LayoutPath;

			page.ExecutePageHierarchy(new WebPageContext(httpContext, base4, model), writer, startPage);
		}

		// Properties
		public string LayoutPath { get; set; }
		public bool RunViewStartPages { get; set; }
		public IEnumerable<string> ViewStartFileExtensions { get; set; }
		internal StartPageLookupDelegate StartPageLookup { get; set; }
	}
}
