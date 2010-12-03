<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<%@ Import Namespace="Signum.Web" %>
<%@ Import Namespace="Signum.Engine" %>
<%@ Import Namespace="Signum.Entities" %>
<%@ Import Namespace="Signum.Utilities" %>
<%@ Import Namespace="Signum.Entities.Authorization" %>
<%@ Import Namespace="Signum.Web.Authorization" %>
<%@ Import Namespace="Signum.Web.Extensions.Properties" %>

<%: Html.DynamicCss("~/authAdmin/Content/authAdmin.css") %>

<%
    using (var tc = Html.TypeContext<PropertyRulePack>())
    {
        Html.EntityLine(tc, f => f.Role);
        Html.ValueLine(tc, f => f.DefaultRule, vl => { vl.UnitText = tc.Value.DefaultLabel; });        
        Html.EntityLine(tc, f => f.Type);
%>

<table class="ruleTable" id="properties">
    <thead>
        <tr>
            <th>
                <%: Resources.PropertiesAscx_Property %>
            </th>
            <th>
                <%: Resources.PropertiesAscx_Modify %>
            </th>
            <th>
                <%: Resources.PropertiesAscx_Read %>
            </th>
            <th>
                <%: Resources.PropertiesAscx_None %>
            </th>
            <th>
                <%: Resources.PropertiesAscx_Overriden %>
            </th>
        </tr>
    </thead>
    <%
        foreach (var item in tc.TypeElementContext(p => p.Rules))
        {
    %>
    <tr>
        <td>
            <%=Html.Span(null, item.Value.Resource.Path)%>
            <%=Html.Hidden(item.Compose("Resource_Path"), item.Value.Resource.Path)%>
            <%=Html.Hidden(item.Compose("AllowedBase"), item.Value.AllowedBase)%>
        </td>
        <td>
            <a class="cbLink modify">
            <%=Html.RadioButton(item.Compose("Allowed"), "Modify", item.Value.Allowed == PropertyAllowed.Modify)%>
            </a>
        </td>
        <td>
            <a class="cbLink read">
            <%=Html.RadioButton(item.Compose("Allowed"), "Read", item.Value.Allowed == PropertyAllowed.Read)%>
            </a>
        </td>
        <td>
            <a class="cbLink none">
            <%=Html.RadioButton(item.Compose("Allowed"), "None", item.Value.Allowed == PropertyAllowed.None)%>
            </a>
        </td>
        <td>
            <%= Html.CheckBox(item.Compose("Overriden"), item.Value.Overriden, new {disabled = "disabled",  @class = "overriden"}) %>
        </td>
    </tr>
    <%
        }
    %>
</table>
<%
    }
%>
