﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.42000
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ASP
{
    using System;
    using System.Collections.Generic;
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
    
    #line 1 "..\..\Mailing\Views\EmailReport.cshtml"
    using Signum.Entities.Mailing;
    
    #line default
    #line hidden
    using Signum.Utilities;
    using Signum.Web;
    
    #line 2 "..\..\Mailing\Views\EmailReport.cshtml"
    using Signum.Web.Mailing;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Mailing/Views/EmailReport.cshtml")]
    public partial class _Mailing_Views_EmailReport_cshtml : System.Web.Mvc.WebViewPage<dynamic>
    {
        public _Mailing_Views_EmailReport_cshtml()
        {
        }
        public override void Execute()
        {
            
            #line 3 "..\..\Mailing\Views\EmailReport.cshtml"
 using (var sc = Html.TypeContext<EmailReportEntity>())
{
    
            
            #line default
            #line hidden
            
            #line 5 "..\..\Mailing\Views\EmailReport.cshtml"
Write(Html.ValueLine(sc,s=>s.Name));

            
            #line default
            #line hidden
            
            #line 5 "..\..\Mailing\Views\EmailReport.cshtml"
                                 
    
            
            #line default
            #line hidden
            
            #line 6 "..\..\Mailing\Views\EmailReport.cshtml"
Write(Html.EntityLine(sc, s => s.EmailTemplate, el => el.AttachFunction = MailingClient.Module["attachEmailReportTemplate"](el,
    sc.SubContextPrefix(a => a.Target),
    Url.Action((MailingController c) => c.GetEmailTemplateEntityImplementations()))));

            
            #line default
            #line hidden
            
            #line 8 "..\..\Mailing\Views\EmailReport.cshtml"
                                                                                    
    
            
            #line default
            #line hidden
            
            #line 9 "..\..\Mailing\Views\EmailReport.cshtml"
Write(Html.EntityLine(sc, s => s.Target, el => { el.Autocomplete = true; el.Find = true; }));

            
            #line default
            #line hidden
            
            #line 9 "..\..\Mailing\Views\EmailReport.cshtml"
                                                                                          
}

            
            #line default
            #line hidden
        }
    }
}
#pragma warning restore 1591
