<%@ Control Language="C#" Inherits="Microsoft.Web.DynamicData.Mvc.MvcFieldTemplate" %>
<script runat="server">
    private string CheckedAttribute {
        get {
            if (FieldValue == null || !((bool)FieldValue))
                return String.Empty;
            return "Checked=\"Checked\"";
        }
    }
</script>
<input type="checkbox" disabled="disabled" <%= CheckedAttribute %> />