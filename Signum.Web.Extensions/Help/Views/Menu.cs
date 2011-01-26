﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
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
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("MvcRazorClassGenerator", "1.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Help/Views/Menu.cshtml")]
    public class _Page_Help_Views_Menu_cshtml : System.Web.Mvc.WebViewPage<dynamic>
    {
#line hidden

        public _Page_Help_Views_Menu_cshtml()
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

Write(Html.ScriptsJs("~/help/scripts/help.js"));

WriteLiteral("\r\n\r\n<div class=\"grid_16\" id=\"syntax-help\">\r\n    <div id=\"syntax-list\">\r\n        U" +
"tiliza la siguiente sintaxis:\r\n        <h2>Textos</h2>\r\n        <table>\r\n       " +
"     <tr><td><b>Texto en negrita</b></td><td>\'\'\'Texto en negrita\'\'\'</td></tr>\r\n " +
"           <tr><td><i>Texto en cursiva</i></td><td>\'\'Texto en cursiva\'\'</td></tr" +
">\r\n            <tr><td><u>Texto subrayado</u></td><td>_Texto subrayado_</td></tr" +
">\r\n            <tr><td><s>Texto tachado</s></td><td>-Texto tachado-</td></tr>\r\n " +
"       </table>\r\n        <h2>Listas</h2>\r\n        <table>\r\n            <tr><td><" +
"ul><li>Elemento no numerado de lista</li><li>Otro elemento</li></ul></td><td>* E" +
"lemento no numerado de lista<br />* Otro elemento</td></tr>\r\n            <tr><td" +
"><ol><li>Elemento numerado de lista</li><li>Otro elemento</li></ol></td><td># El" +
"emento numerado de lista <br /># Otro elemento</td></tr>\r\n        </table>\r\n    " +
"    <h2>Enlaces</h2>\r\n        <table>\r\n            <tr><td><a href=\"#\">Enlace a " +
"entidad</a></td><td>[e:EntidadDN]</td></tr>\r\n            <tr><td><a href=\"#\">Enl" +
"ace a propiedad</a></td><td>[p:EntidadDN.Propiedad]</td></tr>\r\n            <tr><" +
"td><a href=\"#\">Enlace a consulta</a></td><td>[q:EntidadQuery.ObtenerDatos]</td><" +
"/tr>                        \r\n            <tr><td><a href=\"#\">Enlace a operacion" +
"</a></td><td>[o:EntidadOperation.Crear]</td></tr>\r\n            <tr><td><a href=\"" +
"#\">Enlace a espacio de nombres</a></td><td>[n:Negocio.Entities.Bancos]</td></tr>" +
"            \r\n            <tr><td><a href=\"http://www.google.es\">http://www.goog" +
"le.es</a></td><td>[h:http://www.google.es]</td></tr>\r\n            <tr><td><a hre" +
"f=\"#\">Enlace relativo a wiki</a></td><td>[w:Portada]</td></tr>\r\n        </table>" +
"\r\n        Los enlaces admiten un parámetro adicional con el texto que se mostrar" +
"á en el enlace. Ejemplo: <a href=\"http://www.google.es\">Web de Google</a> con [h" +
":http://www.google.es|Web de Google]\r\n        <h2>Imágenes</h2>\r\n        <table>" +
"\r\n        <tr><td>Insertar imagen</td><td>[imageright|Pie de foto|imagen.jpg] o " +
"[imageright|imagen.jpg] (Opciones imageright, imageleft, imagecentre, imageauto)" +
"</td></tr>                        \r\n        </table>\r\n        <h2>Títulos</h2>\r\n" +
"        <table>\r\n            <tr><td><h2>Título nivel 2</h2></td><td>==Título ni" +
"vel 2==</td></tr>\r\n            <tr><td><h3>Título nivel 3</h3></td><td>===Título" +
" nivel 3===</td></tr>\r\n            <tr><td><h4>Título nivel 4</h4></td><td>====T" +
"ítulo nivel 4====</td></tr>\r\n            <tr><td><h5>Título nivel 5</h5></td><td" +
">=====Título nivel 5=====</td></tr>\r\n        </table>\r\n        Los enlaces admit" +
"en un parámetro adicional con el texto que se mostrará en el enlace. Ejemplo: <a" +
" href=\"http://www.google.es\">Web de Google</a> con [h:http://www.google.es|Web d" +
"e Google]\r\n    </div>\r\n</div>\r\n<div class=\"clear\"></div>\r\n<div class=\"grid_12\" s" +
"tyle=\"min-height:1px;\">\r\n    <div id=\"saving-error\"><img src=\"");


                                Write(Url.Content("~/help/Images/icon-warning.png"));

WriteLiteral(@""" /><span class=""text""></span></div> 
</div>
<div class=""grid_4"">
   <!-- <a id=""refresh"" href=""javascript:location.reload(true);"">Refresca la página para wikificar correctamente el texto modificado</a> -->
    <a id=""edit-action"" class=""action"" href=""javascript:SF.Help.edit();"">Editar</a>
    <a id=""syntax-action"" class=""action"" style=""display: none"">Sintaxis</a>    
    <a id=""save-action"" class=""action"" href=""javascript:SF.Help.save();"" style=""display: none"">Guardar</a>    
</div>
<div class=""clear""></div>");


        }
    }
}
