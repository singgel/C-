<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%
    if (Request.IsAuthenticated) {
%>
        欢迎您，<b><%: Page.User.Identity.Name %></b>!
        [ <%: Html.ActionLink("注销", "LogOff", "Account") %> ]
<%
    }
    else {
%> 
        [ <%: Html.ActionLink("登录", "LogOn", "Account") %> ]
<%
    }
%>
