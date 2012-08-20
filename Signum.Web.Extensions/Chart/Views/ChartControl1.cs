﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17626
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
    using Signum.Web;
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
    using Signum.Web.Extensions.Properties;
    using Signum.Entities.DynamicQuery;
    using Signum.Engine.DynamicQuery;
    using Signum.Entities.Reflection;
    using Signum.Entities.Chart;
    using Signum.Web.Chart;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("MvcRazorClassGenerator", "1.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Chart/Views/ChartControl.cshtml")]
    public class _Page_Chart_Views_ChartControl_cshtml : System.Web.Mvc.WebViewPage<TypeContext<ChartRequest>>
    {


        public _Page_Chart_Views_ChartControl_cshtml()
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










WriteLiteral("\r\n");


Write(Html.ScriptCss("~/Chart/Content/SF_Chart.css"));

WriteLiteral("\r\n\r\n");


Write(Html.ScriptsJs("~/Chart/Scripts/SF_Chart.js",
                "~/scripts/d3.v2.min.js"));

WriteLiteral("\r\n\r\n");


   
    QueryDescription queryDescription = (QueryDescription)ViewData[ViewDataKeys.QueryDescription];
    List<FilterOption> filterOptions = (List<FilterOption>)ViewData[ViewDataKeys.FilterOptions];
    bool viewable = (bool)ViewData[ViewDataKeys.View];
    
    var entityColumn = queryDescription.Columns.SingleEx(a => a.IsEntity);
    Type entitiesType = Lite.Extract(entityColumn.Type);


WriteLiteral("\r\n<div id=\"");


    Write(Model.Compose("sfChartControl"));

WriteLiteral("\" \r\n    class=\"sf-search-control sf-chart-control\" \r\n    data-subtokens-url=\"");


                   Write(Url.Action("NewSubTokensCombo", "Chart"));

WriteLiteral("\" \r\n    data-add-filter-url=\"");


                    Write(Url.Action("AddFilter", "Chart"));

WriteLiteral("\" \r\n    data-prefix=\"");


            Write(Model.ControlID);

WriteLiteral("\" >\r\n\r\n    ");


Write(Html.HiddenRuntimeInfo(Model));

WriteLiteral("\r\n\r\n    ");


Write(Html.Hidden(Model.Compose("sfOrders"), Model.Value.Orders.IsNullOrEmpty() ? "" :
        (Model.Value.Orders.ToString(oo => (oo.OrderType == OrderType.Ascending ? "" : "-") + oo.Token.FullKey(), ";") + ";")));

WriteLiteral("\r\n\r\n    <div>\r\n        <div class=\"sf-fields-list\">\r\n            <div class=\"ui-w" +
"idget sf-filters\">\r\n                <div class=\"ui-widget-header ui-corner-top s" +
"f-filters-body\">\r\n                    ");


               Write(Html.ChartRootTokens(Model.Value, queryDescription, Model));

WriteLiteral("\r\n                    \r\n                    ");


               Write(Html.Href(
                            Model.Compose("btnAddFilter"),
                            Signum.Web.Properties.Resources.FilterBuilder_AddFilter,
                            "",
                            Signum.Web.Properties.Resources.Signum_selectToken,
                            "sf-query-button sf-add-filter sf-disabled",
                            new Dictionary<string, object> 
                            { 
                                { "data-icon", "ui-icon-arrowthick-1-s" },
                                { "data-url", Url.SignumAction("AddFilter") }
                            }));

WriteLiteral("\r\n                </div>  \r\n");


                   
                    Html.RenderPartial(Navigator.Manager.FilterBuilderView); 
                

WriteLiteral("            </div>\r\n        </div>\r\n    </div>\r\n\r\n        <div id=\"");


            Write(Model.Compose("sfChartBuilderContainer"));

WriteLiteral("\">\r\n            ");


       Write(Html.Partial(ChartClient.ChartBuilderView, Model.Value));

WriteLiteral("\r\n        </div>\r\n        <script type=\"text/javascript\">\r\n            $(\'#");


           Write(Model.Compose("sfChartBuilderContainer"));

WriteLiteral("\').chartBuilder($.extend({ prefix: \'");


                                                                                        Write(Model.ControlID);

WriteLiteral("\' }, ");


                                                                                                             Write(MvcHtmlString.Create(Model.Value.ToJS()));

WriteLiteral("));\r\n        </script>\r\n    <div class=\"sf-query-button-bar\">\r\n        <button ty" +
"pe=\"submit\" class=\"sf-query-button sf-chart-draw\" data-icon=\"ui-icon-pencil\" id=" +
"\"");


                                                                                              Write(Model.Compose("qbDraw"));

WriteLiteral("\" data-url=\"");


                                                                                                                                   Write(Url.Action<ChartController>(cc => cc.Draw(Model.ControlID)));

WriteLiteral("\">");


                                                                                                                                                                                                  Write(Resources.Chart_Draw);

WriteLiteral("</button>\r\n        ");


   Write(UserChartClient.GetChartMenu(this.ViewContext, queryDescription.QueryName, entitiesType, Model.ControlID).ToString(Html));

WriteLiteral("\r\n    </div>\r\n    \r\n    <div class=\"clearall\"></div>\r\n\r\n    <div id=\"");


        Write(Model.Compose("divResults"));

WriteLiteral("\" class=\"ui-widget ui-corner-all sf-search-results-container\">\r\n");


           Html.RenderPartial(ChartClient.ChartResultsView); 

WriteLiteral("    </div>\r\n\r\n     <fieldset class=\"sf-chart-code\" >\r\n        <legend>Code</legen" +
"d>\r\n          <button type=\"submit\" class=\"sf-query-button sf-save-script\" data-" +
"icon=\"ui-icon-pencil\" id=\"");


                                                                                                 Write(Model.Compose("qbSaveScript"));

WriteLiteral("\" data-url=\"");


                                                                                                                                            Write(Url.Action<ChartController>(cc => cc.SaveScript()));

WriteLiteral("\">");


                                                                                                                                                                                                  Write(Signum.Web.Properties.Resources.Save);

WriteLiteral("</button>\r\n      \r\n        <div class=\"sf-chart-code-container\">\r\n            <te" +
"xtarea rows=\"60\">");


                           Write(Model.Value.ChartScript.Script);

WriteLiteral("</textarea>\r\n        </div>\r\n");


           MvcHtmlString divSelector = MvcHtmlString.Create("#" + Model.Compose("sfChartContainer") + " > .sf-chart-container"); 

WriteLiteral("        <script type=\"text/javascript\">\r\n                (function() {\r\n         " +
"           var $myChart = SF.Chart.getFor(\'");


                                               Write(Model.ControlID);

WriteLiteral("\');\r\n                    $myChart.initOrders();\r\n\r\n                    var $chart" +
"Container = $(\'");


                                        Write(divSelector);

WriteLiteral("\');\r\n                    $chartContainer.closest(\'.sf-tabs\').bind(\"tabsshow\", fun" +
"ction(event, ui) {\r\n                        if (ui.panel.id == \'");


                                        Write(Model.Compose("sfChartContainer"));

WriteLiteral("\') {\r\n                            var data = ");


                                  Write(Html.Json());

WriteLiteral(@";
                            $chartContainer.data(""data"", data);
                            $myChart.reDraw($chartContainer, false);
                        }
                    });
                })();
        </script>
    </fieldset>
</div>
");


        }
    }
}
