﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.239
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ASP
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Web;
    using System.Web.Helpers;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.WebPages;
    using System.Web.Mvc;
    using System.Web.Mvc.Ajax;
    using System.Web.Mvc.Html;
    using System.Web.Routing;
    using Signum.Utilities;
    using Signum.Entities;
    using Signum.Entities.Authorization;
    using Signum.Web;
    using Signum.Web.Auth;
    using Signum.Web.Extensions.Properties;
    using System.Collections;
    using System.Collections.Specialized;
    using System.ComponentModel.DataAnnotations;
    using System.Configuration;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web.Caching;
    using System.Web.DynamicData;
    using System.Web.SessionState;
    using System.Web.Profile;
    using System.Web.UI.WebControls;
    using System.Web.UI.WebControls.WebParts;
    using System.Web.UI.HtmlControls;
    using System.Xml.Linq;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("MvcRazorClassGenerator", "1.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Auth/Views/LoginUserControl.cshtml")]
    public class _Page_Auth_Views_LoginUserControl_cshtml : System.Web.Mvc.WebViewPage<dynamic>
    {


        public _Page_Auth_Views_LoginUserControl_cshtml()
        {
        }
        protected System.Web.HttpApplication ApplicationInstance
        {
            get
            {
                return ((System.Web.HttpApplication)(Context.ApplicationInstance));
            }
        }
        public override void Execute()
        {

 if (UserDN.Current != null && !UserDN.Current.Is(Signum.Engine.Authorization.AuthLogic.AnonymousUser) )
{

WriteLiteral("    ");

WriteLiteral("Usuario: <span class=\"sf-auth-username\">");


                                         Write(UserDN.Current);

WriteLiteral("</span>\r\n");



WriteLiteral("    <span class=\"sf-auth-separator\"> | </span>");


                                              
                                         Write(Html.ActionLink("Logout", "Logout", "Auth", null, new { @class = "sf-logout" }));

                                                                                                                              
}
else
{ 
    
Write(Html.ActionLink("Login", "Login", "Auth", null, new { @class = "sf-login" }));

                                                                                 
}


        }
    }
}
