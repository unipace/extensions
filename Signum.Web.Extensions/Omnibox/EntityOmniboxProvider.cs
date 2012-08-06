﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Signum.Entities.Omnibox;
using Signum.Engine.Maps;
using System.Web.Mvc;
using Signum.Utilities;
using Signum.Entities;

namespace Signum.Web.Omnibox
{
    public class EntityOmniboxProvider : OmniboxClient.OmniboxProvider<EntityOmniboxResult>
    {
        public override OmniboxResultGenerator<EntityOmniboxResult> CreateGenerator()
        {
            return new EntityOmniboxResultGenenerator(Schema.Current.Tables.Keys);
        }

        public override MvcHtmlString RenderHtml(EntityOmniboxResult result)
        {
            MvcHtmlString html = "{0} ".FormatHtml(result.TypeMatch.ToHtml());

            if (result.Id == null && result.ToStr == null)
            {
                html = html.Concat(new MvcHtmlString("..."));
            }
            else
            {
                if (result.Id != null)
                {
                    html = html.Concat("{0}: {1}".FormatHtml(result.Id.ToString(), (result.Lite == null) ? 
                        ColoredSpan(Signum.Entities.Extensions.Properties.Resources.NotFound, "gray") :
                        new HtmlTag("span").InnerHtml(new MvcHtmlString(result.Lite.TryToString()))));
                }
                else
                {
                    if (result.Lite == null)
                    {
                        html = html.Concat("'{0}': {1}".FormatHtml(result.ToStr, 
                            ColoredSpan(Signum.Entities.Extensions.Properties.Resources.NotFound, "gray")));
                    }
                    else
                    {
                        html = html.Concat("{0}: {1}".FormatHtml(result.Lite.Id.ToString(),
                            result.ToStrMatch.ToHtml()));
                    }
                }
            }

            html = html.Concat(ColoredSpan(" ({0})".Formato(Signum.Web.Properties.Resources.View), "dodgerblue"));

            if (result.Lite != null)
                html = new HtmlTag("a")
                    .Attr("href", Navigator.ViewRoute(result.Lite))
                    .InnerHtml(html).ToHtml();

            return html;
        }
    }
}