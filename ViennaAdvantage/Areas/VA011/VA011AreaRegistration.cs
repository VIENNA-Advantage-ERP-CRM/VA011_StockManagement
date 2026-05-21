using System.Web.Mvc;
using System.Web.Optimization;

//NOTE:--    Please replace ViennaAdvantage with prefix of your module..

namespace VA011 //  Please replace namespace with prefix of your module..
{
    public class VA011AreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "VA011";   //Please replace "ViennaAdvantage" with prefix of your module.......
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
              "VA011_default",
              "VA011/{controller}/{action}/{id}",
              new { action = "Index", id = UrlParameter.Optional }
              , new[] { "VA011.Controllers" }
          );     // Please replace ViennaAdvantage with prefix of your module...


            StyleBundle style = new StyleBundle("~/Areas/VA011/Contents/VA011minstyleall.css");
            ScriptBundle script = new ScriptBundle("~/Areas/VA011/Scripts/VA011minall.js");


            //style.Include("~/Areas/VA011/Contents/VA011_Inventory.css");
            //style.Include("~/Areas/VA011/Contents/VA011_style.css");
            //script.Include("~/Areas/VA011/Scripts/apps/forms/inventory.js",
            //           "~/Areas/VA011/Scripts/jquery-barcode.js");

            script.Include("~/Areas/VA011/Scripts/dist/VA011.all.min.js");
            script.Include("~/Areas/VA011/Scripts/dist/VA011React.min.js");
            style.Include("~/Areas/VA011/Contents/VA011.all.min.css");

            script.Transforms.Clear();
            script.Transforms.Add(new NoTransform());

            VAdvantage.ModuleBundles.RegisterScriptBundle(script, "VA011", 10);
            VAdvantage.ModuleBundles.RegisterStyleBundle(style, "VA011", 10);
        }
    }

    public sealed class NoTransform : IBundleTransform
    {
        public void Process(BundleContext context, BundleResponse response)
        {
            // Do nothing: keep content as-is (no minify, no parse)
        }
    }
}