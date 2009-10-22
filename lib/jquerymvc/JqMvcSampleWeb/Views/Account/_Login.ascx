<%@ Control Language="C#" Inherits="JqueryMvc.Mvc.ExtViewUserControl" %>
    <h2>Login</h2>
    <p>
        Please enter your username and password below. If you don't have an account,
        please <%= Ajax.ActionLink("register", "Register", new AjaxOptions() { UpdateTargetId = "maincontent" })%>.
    </p>
    <% using (Ajax.Form("Login", new AjaxOptions { UpdateTargetId = "maincontent" })) { %>
        <div>
            <table>
                <tr>
                    <td>Username:</td>
                    <td><%= Html.TextBox("username") %></td>
                </tr>
                <tr>
                    <td>Password:</td>
                    <td><%= Html.Password("password") %></td>
                </tr>
                <tr>
                    <td></td>
                    <td><input type="checkbox" name="rememberMe" value="true" /> Remember me?</td>
                </tr>
                <tr>
                    <td></td>
                    <td><input type="submit" value="Login" /></td>
                </tr>
            </table>
        </div>
    <% } %>
