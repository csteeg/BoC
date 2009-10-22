<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<script runat="server">
    private string CheckedAttribute {
        get {
            //if (Model == null || !((bool)Model))
                return String.Empty;
            return "Checked=\"Checked\"";
        }
    }
</script>
<input type="checkbox" disabled="disabled" <%= CheckedAttribute %> />