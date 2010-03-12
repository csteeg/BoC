using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace MvcContrib.UI.Grid
{
    /// <summary>
    /// Adds the header row with column name links which carry data to enable sorting.
    /// </summary>
    /// <typeparam name="T">The type of the object in the data source list of the grid to be rendered</typeparam>
    public class DynamicDataGridRenderer<T>: HtmlTableGridRenderer<T> where T : class
    {
        public DynamicDataGridRenderer(ViewEngineCollection engines) : base(engines) {}
        public DynamicDataGridRenderer() {}

        /*protected override void RenderCellValue(GridColumn<T> column, GridRowViewData<T> rowData)
        {
            if (!String.IsNullOrEmpty(column.Name))
            {
                column.DoNotEncode();
                var cellValue = column.GetValue(rowData.Item);

                if (cellValue != null)
                {
                    var template = 
                        MvcContrib.UI.InputBuilder.InputBuilder.Conventions..PartialNameConvention(
                            typeof (T).GetProperty(column.Name));
                    if (!String.IsNullOrEmpty(template))
                    {
                        if (TryRenderPartial(this.Context, "FieldTemplates/" + template, cellValue,
                                             MvcContrib.UI.InputBuilder.InputBuilder.Conventions.Layout(template)))
                            return;

                    }
                }
            }
            base.RenderCellValue(column, rowData);
        }*/

        private static IView FindPartialView(ViewContext viewContext, string partialViewName, ViewEngineCollection viewEngineCollection, string masterName)
        {
            ViewEngineResult result = viewEngineCollection.FindView(viewContext, partialViewName, masterName);
            if (result.View != null)
            {
                return result.View;
            }

            StringBuilder locationsText = new StringBuilder();
            foreach (string location in result.SearchedLocations)
            {
                locationsText.AppendLine();
                locationsText.Append(location);
            }

            return null;
        }

    }
}