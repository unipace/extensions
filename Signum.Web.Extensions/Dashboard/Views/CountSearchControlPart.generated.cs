﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Signum.Web.Extensions.Dashboard.Views
{
    using System;
    using System.Collections.Generic;
    
    #line 1 "..\..\Dashboard\Views\CountSearchControlPart.cshtml"
    using System.Configuration;
    
    #line default
    #line hidden
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Web;
    using System.Web.Helpers;
    using System.Web.Mvc;
    using System.Web.Mvc.Ajax;
    using System.Web.Mvc.Html;
    using System.Web.Routing;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.WebPages;
    using Signum.Entities;
    
    #line 2 "..\..\Dashboard\Views\CountSearchControlPart.cshtml"
    using Signum.Entities.Dashboard;
    
    #line default
    #line hidden
    
    #line 4 "..\..\Dashboard\Views\CountSearchControlPart.cshtml"
    using Signum.Entities.DynamicQuery;
    
    #line default
    #line hidden
    using Signum.Utilities;
    using Signum.Web;
    
    #line 3 "..\..\Dashboard\Views\CountSearchControlPart.cshtml"
    using Signum.Web.Dashboard;
    
    #line default
    #line hidden
    
    #line 5 "..\..\Dashboard\Views\CountSearchControlPart.cshtml"
    using Signum.Web.UserQueries;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Dashboard/Views/CountSearchControlPart.cshtml")]
    public partial class CountSearchControlPart : System.Web.Mvc.WebViewPage<dynamic>
    {
        public CountSearchControlPart()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n");

            
            #line 7 "..\..\Dashboard\Views\CountSearchControlPart.cshtml"
 using (var tc = Html.TypeContext<CountSearchControlPartDN>())
{

            
            #line default
            #line hidden
WriteLiteral("    <div");

WriteLiteral(" class=\"sf-cp-count-uq-list\"");

WriteLiteral(">\r\n");

            
            #line 10 "..\..\Dashboard\Views\CountSearchControlPart.cshtml"
        
            
            #line default
            #line hidden
            
            #line 10 "..\..\Dashboard\Views\CountSearchControlPart.cshtml"
         for (int i = 0; i < tc.Value.UserQueries.Count; i++)
        {
            CountUserQueryElementDN uq = tc.Value.UserQueries[i];
            object queryName = Navigator.Manager.QuerySettings.Keys.FirstEx(k => QueryUtils.GetQueryUniqueKey(k) == uq.UserQuery.Query.Key);
            FindOptions fo = new FindOptions(queryName)
            {
                ShowFilters = false
            };

            fo.ApplyUserQuery(uq.UserQuery);

            
            #line default
            #line hidden
WriteLiteral("            <p>\r\n");

WriteLiteral("                ");

            
            #line 21 "..\..\Dashboard\Views\CountSearchControlPart.cshtml"
           Write(Html.CountSearchControlSpan(fo, new Context(tc, "{0}_cnt".Formato(i)), csc =>
           {
               csc.Navigate = true;
               csc.Href = uq.Href;
               csc.QueryLabelText = uq.Label.DefaultText(uq.UserQuery.ToString());
           }));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </p>\r\n");

            
            #line 28 "..\..\Dashboard\Views\CountSearchControlPart.cshtml"
        }

            
            #line default
            #line hidden
WriteLiteral("    </div>\r\n");

            
            #line 30 "..\..\Dashboard\Views\CountSearchControlPart.cshtml"
}
            
            #line default
            #line hidden
        }
    }
}
#pragma warning restore 1591
