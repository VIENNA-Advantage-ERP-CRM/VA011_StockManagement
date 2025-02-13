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


            style.Include("~/Areas/VA011/Contents/VA011_Inventory.css");
            style.Include("~/Areas/VA011/Contents/VA011_style.css");
            script.Include("~/Areas/VA011/Scripts/apps/forms/inventory.js",
                       "~/Areas/VA011/Scripts/jquery-barcode.js");


            //style.Include("~/Areas/VA011/Contents/VA011minstyle.css");
            //script.Include("~/Areas/VA011/Scripts/VA011min.js");

            /*-------------------------------------------------------
                    Here include all js files in style bundle......see example below....
             --------------------------------------------------------*/


            //script.Include("~/Areas/ViennaAdvantage/Scripts/example1.js",
            //               "~/Areas/ViennaAdvantage/Scripts/example2.js");




            /*-------------------------------------------------------
              Please replace "ViennaAdvantage" with prefix of your module..
             * 
             * 1. first parameter is script/style bundle...
             * 
             * 2. Second parameter is module prefix...
             * 
             * 3. Third parameter is order of loading... (dafault is 10 )
             * 
             --------------------------------------------------------*/

            VAdvantage.ModuleBundles.RegisterScriptBundle(script, "VA011", 10);
            VAdvantage.ModuleBundles.RegisterStyleBundle(style, "VA011", 10);
        }
    }
}