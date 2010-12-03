<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Signum.Web" %>
<%@ Import Namespace="Signum.Engine" %>
<%@ Import Namespace="Signum.Entities" %>
<%@ Import Namespace="Signum.Utilities" %>
<%@ Import Namespace="Signum.Web.Authorization" %>
<%@ Import Namespace="Signum.Entities.Authorization" %>
<%@ Import Namespace="Signum.Engine.Operations" %>
<%@ Import Namespace="Signum.Engine.DynamicQuery" %>
<%@ Import Namespace="Signum.Web.Extensions.Properties" %>

<%: Html.DynamicCss("~/authAdmin/Content/authAdmin.css") %>
<%: Html.ScriptsJs("~/authAdmin/Scripts/authAdmin.js") %>

<script>
    $(function() {
    magicCheckBoxes($(document));
        treeView();
    });
</script>

<%
    using (var tc = Html.TypeContext<TypeRulePack>())
    {
        Html.EntityLine(tc, f => f.Role);
        Html.ValueLine(tc, f => f.DefaultRule, vl => { vl.UnitText = tc.Value.DefaultLabel; });    
%>



<table class="ruleTable">
    <thead>
        <tr>
            <th>
                <%: Resources.TypesAscx_Type %>
            </th>
            <th>
                <%: Resources.TypesAscx_Create %>
            </th>
            <th>
                <%: Resources.TypesAscx_Modify %>
            </th>
            <th>
                <%: Resources.TypesAscx_Read %>
            </th>
            <th>
                <%: Resources.TypesAscx_None %>
            </th>
            <th>
                <%: Resources.TypesAscx_Overriden %>
            </th>
            <% if (Navigator.Manager.EntitySettings.ContainsKey(typeof(PermissionRulePack)))
               { %>
            <th>
                <%: Resources.TypesAscx_Properties %>
            </th>
            <%} %>
            <% if (Navigator.Manager.EntitySettings.ContainsKey(typeof(OperationRulePack)))
               { %>
            <th>
                <%: Resources.TypesAscx_Operations %>
            </th>
            <%} %>
            <% if (Navigator.Manager.EntitySettings.ContainsKey(typeof(QueryRulePack)))
               { %>
            <th>
                <%: Resources.TypesAscx_Queries %>
            </th>
            <%} %>
        </tr>
    </thead>
    <%
        foreach (var iter in tc.TypeElementContext(p => p.Rules).GroupBy(a => a.Value.Resource.Namespace).OrderBy(a => a.Key).Iterate())
        {
    %>
    <tr>
        <td colspan="6">
            <a class="namespace">
                <div class="treeView <%=iter.IsLast?"tvExpandedLast": "tvExpanded" %>">
                </div>
                <img src="<%: Url.Content("~/authAdmin/Images/namespace.png") %>" />
                <%=Html.Span(null, iter.Value.Key, "namespace") %>
            </a>
        </td>
    </tr>
    <%
        foreach (var iter2 in iter.Value.OrderBy(a => a.Value.Resource.FriendlyName).Iterate())
        {
            var item = iter2.Value;
  
    %>
    <tr>
        <td>
            <div class="treeView <%=iter.IsLast?"tvBlank": "tvLine" %>">
            </div>
            <div class="treeView <%=iter2.IsLast?"tvLeafLast": "tvLeaf" %>">
            </div>
            <img src="<%: Url.Content("~/authAdmin/Images/class.png") %>" />
            <%=Html.Span(null, item.Value.Resource.FriendlyName)%>
            <%=Html.HiddenRuntimeInfo(item, i => i.Resource)%>
            <%=Html.Hidden(item.Compose("AllowedBase"), item.Value.AllowedBase.ToStringParts())%>
            <%=Html.Span(null, iter.Value.Key, "namespace", new Dictionary<string, object> { {"style", "display:none"}})%>
        </td>
        <td>
            <a class="cbLink create">
                <%=Html.CheckBox(item.Compose("Allowed_Create"), item.Value.Allowed.IsActive(TypeAllowedBasic.Create), new { tag = "Create" })%>
            </a>
        </td>
        <td>
            <a class="cbLink modify">
                <%=Html.CheckBox(item.Compose("Allowed_Modify"), item.Value.Allowed.IsActive(TypeAllowedBasic.Modify), new { tag = "Modify" })%>
            </a>
        </td>
        <td>
            <a class="cbLink read">
                <%=Html.CheckBox(item.Compose("Allowed_Read"), item.Value.Allowed.IsActive(TypeAllowedBasic.Read), new { tag = "Read" })%>
            </a>
        </td>
        <td>
            <a class="cbLink none">
                <%=Html.CheckBox(item.Compose("Allowed_None"), item.Value.Allowed.IsActive(TypeAllowedBasic.None), new { tag = "None" })%>
            </a>
        </td>
        <td>
            <%= Html.CheckBox(item.Compose("Overriden"), item.Value.Overriden, new {disabled = "disabled",  @class = "overriden"}) %>
        </td>
        <% if (Navigator.Manager.EntitySettings.ContainsKey(typeof(PropertyRulePack)))
           { 
        %>
        <td>
            <%if (item.Value.Properties.HasValue)
              {%>
            <a href="javascript:openDialog('AuthAdmin/Properties', {role:<%=tc.Value.Role.Id%>, type:<%=item.Value.Resource.Id%>});">
                <div class="property">
                    <div class="thumb <%=item.Value.Properties.ToString().ToLower()%>">
                    </div>
                </div>
            </a>
            <%} %>
        </td>
        <%} %>
        <% if (Navigator.Manager.EntitySettings.ContainsKey(typeof(OperationRulePack)))
           { 
        %>
        <td>
            <%if (item.Value.Operations.HasValue)
              {%>
            <a href="javascript:openDialog('AuthAdmin/Operations', {role:<%=tc.Value.Role.Id%>, type:<%=item.Value.Resource.Id%>});">
                <div class="operation">
                    <div class="thumb <%=item.Value.Operations.ToString().ToLower()%>">
                    </div>
                </div>
            </a>
            <%} %>
        </td>
        <%} %>
        <% if (Navigator.Manager.EntitySettings.ContainsKey(typeof(QueryRulePack)))
           {
        %>
        <td>
            <%if (item.Value.Queries.HasValue)
              {%>
            <a href="javascript:openDialog('AuthAdmin/Queries', {role:<%=tc.Value.Role.Id%>, type:<%=item.Value.Resource.Id%>});">
                <div class="query">
                    <div class="thumb <%=item.Value.Queries.ToString().ToLower()%>">
                    </div>
                </div>
            </a>
            <%} %>
        </td>
        <%} %>
    </tr>
    <%
        }
        }
    %>
</table>
<%
    }
%>
