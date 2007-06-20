<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ListOfProfiles.ascx.cs" Inherits="profiles_ListOfProfiles" %>
<div class="profilegridholder">
    <div class="profilesgrid">
        <asp:Xml runat="server" 
                ID="xmlControl"
                TransformSource="profilesgrid.xsl"
         />
   </div>
   <div class="profilesgrid-pager">
        <a class="previous" href='<%# "listprofiles.aspx?" + this.ClientID + ".startRow=" + (this.StartRow - 5) %>' runat="server" visible="<%# this.StartRow > 0 %>">&lt;&lt; Previous</a>
        <a class="next" href="listprofiles.aspx?<%# this.ClientID %>.startRow=<%# this.StartRow + 5 %>">Next &gt;&gt;</a>
   </div>
</div>