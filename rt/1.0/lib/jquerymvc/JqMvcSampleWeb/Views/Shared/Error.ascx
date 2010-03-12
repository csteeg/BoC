<%@ Control Language="C#" CodeBehind="Error.ascx.cs" Inherits="JqMvcSampleWeb.Views.Shared.Error" %>
<div class="jqmvc-errors">
    <ul>
        <li><h2>Sorry, an error occurred while processing your request.</h2></li>
    <% if (!ViewContext.HttpContext.IsCustomErrorEnabled) { %>
        <h3>
            Exception details:
        </h3>
        <div style="overflow: auto;">
            <%
                Stack<Exception> exceptions = new Stack<Exception>();
                for (Exception ex = ViewData.Model.Exception; ex != null; ex = ex.InnerException) {
                    exceptions.Push(ex);
                }
                foreach (Exception ex in exceptions) {
                    %>
                        <div>
                            <b><%= Html.Encode(ex.GetType().FullName)%></b>: <%= Html.Encode(ex.Message)%>
                        </div>
                        <div>
                            <pre style="font-size: medium;"><%= Html.Encode(ex.StackTrace)%></pre>
                        </div>
                    <%
                }  
            %>
        </div>
    <% } %>
    </ul>
</div>