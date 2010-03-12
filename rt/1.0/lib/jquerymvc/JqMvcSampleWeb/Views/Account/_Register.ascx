﻿<%@ Control Language="C#" Inherits="JqueryMvc.Mvc.ExtViewUserControl" %>
    <h2>Account Creation</h2>
    <p>
        Use the form below to create a new account. 
    </p>
    <p>
        Passwords are required to be a minimum of <%=Html.Encode(ViewData["PasswordLength"])%> characters in length.
    </p>
    <% using (Ajax.Form("Register", new AjaxOptions { UpdateTargetId = "maincontent" }))
       { %>
        <div>
            <table>
                <tr>
                    <td>Username:</td>
                    <td><%= Html.TextBox("username")%></td>
                </tr>
                <tr>
                    <td>Email:</td>
                    <td><%= Html.TextBox("email")%></td>
                </tr>
                <tr>
                    <td>Password:</td>
                    <td><%= Html.Password("password")%></td>
                </tr>
                <tr>
                    <td>Confirm password:</td>
                    <td><%= Html.Password("confirmPassword")%></td>
                </tr>
                <tr>
                    <td></td>
                    <td><input type="submit" value="Register" /></td>
                </tr>
            </table>
        </div>
    <% } %>
